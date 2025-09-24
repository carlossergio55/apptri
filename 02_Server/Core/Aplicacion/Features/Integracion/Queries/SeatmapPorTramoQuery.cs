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
    public class SeatmapPorTramoQuery : IRequest<Response<List<SeatmapSeatDto>>>
    {
        public int ViajeId { get; set; }
        public int OrigenParadaId { get; set; }
        public int DestinoParadaId { get; set; }
        public int ReservaTtlMinutos { get; set; } = 10; // TTL de bloqueos
    }

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
            if (request.OrigenParadaId == request.DestinoParadaId)
                throw new InvalidOperationException("Origen y destino no pueden ser iguales.");

            var viaje = await _viajeRepo.GetByIdAsync(request.ViajeId);
            if (viaje is null)
                throw new KeyNotFoundException("Viaje no encontrado.");

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

            var asientosAll = await _asientoRepo.ListAsync();
            var asientos = asientosAll
                .Where(a => a.IdBus == viaje.IdBus)
                .Select(a => new { a.IdAsiento, a.Numero })
                .OrderBy(a => a.Numero)
                .ToList();

            // === Cronología LOCAL coherente y ventana T–2h ===
            var ahora = DateTime.Now;
            var salidaLocal = viaje.Fecha.Date + viaje.HoraSalida; // DateOnly + TimeSpan => DateTime local
            var limite = salidaLocal.AddHours(-2);                  // T–2h

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
                    b.FechaReservaUtc,
                    b.FechaCompra
                })
                .ToList();

            var clienteIds = boletos.Select(b => b.IdCliente).Distinct().ToList();
            var clientesAll = await _clienteRepo.ListAsync();
            var clientesMap = clientesAll
                .Where(c => clienteIds.Contains(c.IdCliente))
                .ToDictionary(c => c.IdCliente, c => new { c.Nombre, c.Carnet });

            var boletosPorAsiento = boletos.GroupBy(b => b.IdAsiento)
                                           .ToDictionary(g => g.Key, g => g.ToList());

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
                    var conflictivos = blist
                        .Where(b => b.OrigenParadaId.HasValue && b.DestinoParadaId.HasValue)
                        .Where(b =>
                        {
                            // Solo consideramos PAGADO o BLOQUEADO vigente
                            var estado = (b.Estado ?? "").ToUpperInvariant();
                            if (estado == "ANULADO") return false; // ignora anulados
                            var esPagado = (estado == "PAGADO");

                            var bloqueoVigente = false;
                            if (estado == "BLOQUEADO")
                            {
                                // Bloqueo solo cuenta si estamos fuera de T–2h y TTL no venció
                                var baseDate = b.FechaReservaUtc ?? b.FechaCompra ?? ahora;
                                var minutos = (ahora - baseDate).TotalMinutes;
                                bloqueoVigente = (ahora < limite) && (minutos <= request.ReservaTtlMinutos);
                            }

                            if (!esPagado && !bloqueoVigente) return false;

                            if (!orden.ContainsKey(b.OrigenParadaId!.Value) || !orden.ContainsKey(b.DestinoParadaId!.Value))
                                return false;

                            var oE = orden[b.OrigenParadaId!.Value];
                            var dE = orden[b.DestinoParadaId!.Value];
                            // solape de tramos [oN,dN) vs [oE,dE)
                            return Math.Max(oE, oN) < Math.Min(dE, dN);
                        })
                        .ToList();

                    if (conflictivos.Count > 0)
                    {
                        var activo = conflictivos.FirstOrDefault(x => string.Equals(x.Estado, "PAGADO", StringComparison.OrdinalIgnoreCase))
                                     ?? conflictivos.FirstOrDefault(x => string.Equals(x.Estado, "BLOQUEADO", StringComparison.OrdinalIgnoreCase));

                        if (activo is not null)
                        {
                            dto.EstadoSeat = string.Equals(activo.Estado, "PAGADO", StringComparison.OrdinalIgnoreCase) ? "OCUPADO" : "RESERVADO";
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

            return new Response<List<SeatmapSeatDto>>(seatDtos, "Seatmap generado.");
        }
    }
}
