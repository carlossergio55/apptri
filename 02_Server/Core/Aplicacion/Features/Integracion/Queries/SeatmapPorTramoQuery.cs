using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

using Aplicacion.Wrappers;
using Aplicacion.Interfaces;                 // IRepositoryAsync<T>
using Dominio.Entities.Integracion;         // Viaje, RutaParada, Asiento, Boleto, Cliente

namespace Aplicacion.Features.Integracion.Queries
{
    // ====== Query (request) ======
    public class SeatmapPorTramoQuery : IRequest<Response<List<SeatmapSeatDto>>>
    {
        public int ViajeId { get; set; }
        public int OrigenParadaId { get; set; }
        public int DestinoParadaId { get; set; }
        /// <summary>Minutos para considerar expirada una reserva BLOQUEADO.</summary>
        public int ReservaTtlMinutos { get; set; } = 10;
    }

    // ====== DTO de respuesta de cada asiento ======
    public class SeatmapSeatDto
    {
        public int IdAsiento { get; set; }
        public int Numero { get; set; }
        /// <summary>LIBRE | RESERVADO | OCUPADO | NO_EXISTE</summary>
        public string EstadoSeat { get; set; } = "LIBRE";
        public int? IdBoleto { get; set; }
        public string? ClienteNombre { get; set; }
        public string? ClienteCI { get; set; }
        public decimal? Precio { get; set; }
        public int? OrigenParadaId { get; set; }
        public int? DestinoParadaId { get; set; }
    }

    // ====== Handler ======
    public class SeatmapPorTramoQueryHandler : IRequestHandler<SeatmapPorTramoQuery, Response<List<SeatmapSeatDto>>>
    {
        private readonly IRepositoryAsync<Viaje> _viajeRepo;
        private readonly IRepositoryAsync<RutaParada> _rutaParadaRepo;
        private readonly IRepositoryAsync<Asiento> _asientoRepo;
        private readonly IRepositoryAsync<Boleto> _boletoRepo;
        private readonly IRepositoryAsync<Cliente> _clienteRepo;

        public SeatmapPorTramoQueryHandler(
            IRepositoryAsync<Viaje> viajeRepo,
            IRepositoryAsync<RutaParada> rutaParadaRepo,
            IRepositoryAsync<Asiento> asientoRepo,
            IRepositoryAsync<Boleto> boletoRepo,
            IRepositoryAsync<Cliente> clienteRepo)
        {
            _viajeRepo = viajeRepo;
            _rutaParadaRepo = rutaParadaRepo;
            _asientoRepo = asientoRepo;
            _boletoRepo = boletoRepo;
            _clienteRepo = clienteRepo;
        }

        public async Task<Response<List<SeatmapSeatDto>>> Handle(SeatmapPorTramoQuery request, CancellationToken ct)
        {
            // 0) Validación básica
            if (request.OrigenParadaId == request.DestinoParadaId)
                throw new InvalidOperationException("Origen y destino no pueden ser iguales.");

            // 1) Viaje (IdRuta, IdBus, Capacidad)
            var viaje = await _viajeRepo.GetByIdAsync(request.ViajeId);
            if (viaje is null)
                throw new KeyNotFoundException("Viaje no encontrado.");

            // 2) Orden de paradas de la ruta
            var rutaParadasAll = await _rutaParadaRepo.ListAsync();
            var rp = rutaParadasAll
                        .Where(x => x.IdRuta == viaje.IdRuta)
                        .OrderBy(x => x.Orden)
                        .Select(x => new { x.IdParada, x.Orden })
                        .ToList();

            var orden = rp.ToDictionary(x => x.IdParada, x => x.Orden);
            if (!orden.ContainsKey(request.OrigenParadaId) || !orden.ContainsKey(request.DestinoParadaId))
                throw new InvalidOperationException("Paradas fuera de la ruta del viaje.");

            var oN = orden[request.OrigenParadaId];
            var dN = orden[request.DestinoParadaId];
            if (oN >= dN)
                throw new InvalidOperationException("El orden de paradas es inválido (origen debe ser antes que destino).");

            // 3) Plantilla de asientos del bus (IdAsiento, Numero)
            var asientosAll = await _asientoRepo.ListAsync();
            var asientos = asientosAll
                .Where(a => a.IdBus == viaje.IdBus)
                .Select(a => new { a.IdAsiento, a.Numero })
                .OrderBy(a => a.Numero)
                .ToList();

            // 4) Boletos del viaje
            var ahora = DateTime.Now;
            var boletosAll = await _boletoRepo.ListAsync();
            var boletos = boletosAll
                .Where(b => b.IdViaje == request.ViajeId)
                .Select(b => new
                {
                    b.IdBoleto,
                    b.IdAsiento,
                    b.Estado,
                    b.OrigenParadaId,
                    b.DestinoParadaId,
                    b.IdCliente,
                    b.Precio,
                    b.FechaCompra
                })
                .ToList();

            // 5) Clientes (solo los necesarios)
            var clienteIds = boletos.Select(b => b.IdCliente).Distinct().ToList();
            var clientesAll = await _clienteRepo.ListAsync();
            var clientesMap = clientesAll
                .Where(c => clienteIds.Contains(c.IdCliente))
                .ToDictionary(c => c.IdCliente, c => new { c.Nombre, c.Carnet });

            // 6) Lookup de boletos por asiento
            var boletosPorAsiento = boletos
                .GroupBy(b => b.IdAsiento)
                .ToDictionary(g => g.Key, g => g.ToList());

            // 7) Construir seatmap
            var seatDtos = new List<SeatmapSeatDto>();

            foreach (var a in asientos)
            {
                var dto = new SeatmapSeatDto
                {
                    IdAsiento = a.IdAsiento,
                    Numero = a.Numero,
                    EstadoSeat = "LIBRE"
                };

                if (boletosPorAsiento.TryGetValue(a.IdAsiento, out var blist))
                {
                    // buscar conflictos de tramo: max(oE,oN) < min(dE,dN)
                    var conflictivos = blist
                        .Where(b => b.OrigenParadaId.HasValue && b.DestinoParadaId.HasValue)
                        .Where(b =>
                        {
                            // reservas expiradas no cuentan
                            bool esBloqueadoExpirado = string.Equals(b.Estado, "BLOQUEADO", StringComparison.OrdinalIgnoreCase) &&
                                                       ((ahora - b.FechaCompra).TotalMinutes > request.ReservaTtlMinutos);
                            if (esBloqueadoExpirado) return false;

                            if (!orden.ContainsKey(b.OrigenParadaId!.Value) || !orden.ContainsKey(b.DestinoParadaId!.Value))
                                return false;

                            var oE = orden[b.OrigenParadaId!.Value];
                            var dE = orden[b.DestinoParadaId!.Value];
                            return Math.Max(oE, oN) < Math.Min(dE, dN);
                        })
                        .ToList();

                    if (conflictivos.Count > 0)
                    {
                        // Prioriza PAGADO sobre BLOQUEADO
                        var pagado = conflictivos.FirstOrDefault(b => string.Equals(b.Estado, "PAGADO", StringComparison.OrdinalIgnoreCase));
                        var activo = pagado ?? conflictivos.FirstOrDefault(b => string.Equals(b.Estado, "BLOQUEADO", StringComparison.OrdinalIgnoreCase));

                        if (activo is not null)
                        {
                            dto.EstadoSeat = (string.Equals(activo.Estado, "PAGADO", StringComparison.OrdinalIgnoreCase)) ? "OCUPADO" : "RESERVADO";
                            dto.IdBoleto = activo.IdBoleto;
                            dto.Precio = activo.Precio;
                            dto.OrigenParadaId = activo.OrigenParadaId;
                            dto.DestinoParadaId = activo.DestinoParadaId;

                            if (clientesMap.TryGetValue(activo.IdCliente, out var c))
                            {
                                dto.ClienteNombre = c.Nombre;
                                dto.ClienteCI = c.Carnet.ToString();
                            }
                        }
                    }
                }

                seatDtos.Add(dto);
            }

            return new Response<List<SeatmapSeatDto>>(seatDtos);
        }
    }
}
