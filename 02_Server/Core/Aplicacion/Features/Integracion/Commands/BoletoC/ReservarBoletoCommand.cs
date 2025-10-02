using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediatR;

using Aplicacion.Wrappers;
using Aplicacion.Interfaces;
using Dominio.Entities.Integracion;

namespace Aplicacion.Features.Integracion.Commands.BoletoC
{
    public class ReservarBoletoCommand : IRequest<Response<int>>
    {
        public int IdViaje { get; set; }
        public int IdAsiento { get; set; }

        // Ahora opcionales: puedes reservar sin cliente ni precio y completarlos al confirmar
        public int? IdCliente { get; set; }
        public decimal? Precio { get; set; }

        public int OrigenParadaId { get; set; }
        public int DestinoParadaId { get; set; }

        // TTL del bloqueo por clic (UI admin/público)
        public int ReservaTtlMinutos { get; set; } = 10;
    }

    public class ReservarBoletoCommandHandler : IRequestHandler<ReservarBoletoCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Viaje> _viajeRepo;
        private readonly IRepositoryAsync<RutaParada> _rutaParadaRepo;
        private readonly IRepositoryAsync<Asiento> _asientoRepo;
        private readonly IRepositoryAsync<Boleto> _boletoRepo;
        private readonly IRepositoryAsync<Cliente> _clienteRepo;

        public ReservarBoletoCommandHandler(
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

        public async Task<Response<int>> Handle(ReservarBoletoCommand request, CancellationToken ct)
        {
            // -------- Validaciones mínimas --------
            if (request.OrigenParadaId == request.DestinoParadaId)
                throw new InvalidOperationException("Origen y destino no pueden ser iguales.");

            var viaje = await _viajeRepo.GetByIdAsync(request.IdViaje);
            if (viaje is null) throw new KeyNotFoundException("Viaje no encontrado.");

            var asiento = await _asientoRepo.GetByIdAsync(request.IdAsiento);
            if (asiento is null) throw new KeyNotFoundException("Asiento no encontrado.");
            if (asiento.IdBus != viaje.IdBus)
                throw new InvalidOperationException("El asiento no pertenece al bus del viaje.");

            // Si envían cliente, valida existencia
            if (request.IdCliente.HasValue && request.IdCliente.Value > 0)
            {
                var cli = await _clienteRepo.GetByIdAsync(request.IdCliente.Value);
                if (cli is null) throw new KeyNotFoundException("Cliente no encontrado.");
            }

            // Validar tramo dentro de la ruta
            var rutaParadasAll = await _rutaParadaRepo.ListAsync();
            var ordenPorParada = rutaParadasAll
                .Where(x => x.IdRuta == viaje.IdRuta)
                .OrderBy(x => x.Orden)
                .ToDictionary(x => x.IdParada, x => x.Orden);

            if (!ordenPorParada.ContainsKey(request.OrigenParadaId) || !ordenPorParada.ContainsKey(request.DestinoParadaId))
                throw new InvalidOperationException("Paradas fuera de la ruta del viaje.");

            var oN = ordenPorParada[request.OrigenParadaId];
            var dN = ordenPorParada[request.DestinoParadaId];
            if (oN >= dN)
                throw new InvalidOperationException("El orden de paradas es inválido (origen debe ser antes que destino).");

            // -------- Tiempo local --------
            var ahoraLocal = DateTime.Now;
            var salidaLocal = viaje.Fecha.Date + viaje.HoraSalida;  // DateOnly + TimeSpan => DateTime
            var dosHorasAntes = salidaLocal.AddHours(-2);

            // -------- Conflicto de asiento por tramo --------
            var boletosAll = await _boletoRepo.ListAsync();

            bool hayConflicto = boletosAll
                .Where(b => b.IdViaje == request.IdViaje && b.IdAsiento == request.IdAsiento)
                .Where(b => b.OrigenParadaId.HasValue && b.DestinoParadaId.HasValue)
                .Where(b =>
                {
                    var estado = (b.Estado ?? "").ToUpperInvariant();

                    // base del TTL
                    var fechaBase = b.FechaReservaUtc ?? b.FechaCompra ?? ahoraLocal;
                    var minutos = (ahoraLocal - fechaBase).TotalMinutes;

                    // Cuenta como bloqueo:
                    //  - siempre si está PAGADO
                    //  - si está BLOQUEADO y su TTL sigue vigente Y aún no estamos en la ventana T–2h
                    var bloqueaPorEstado =
                           estado == "PAGADO"
                        || (estado == "BLOQUEADO" && (minutos <= request.ReservaTtlMinutos) && (ahoraLocal < dosHorasAntes));

                    if (!bloqueaPorEstado) return false;

                    // Solape de tramos [oN,dN) vs [oE,dE)
                    var oE = ordenPorParada[b.OrigenParadaId!.Value];
                    var dE = ordenPorParada[b.DestinoParadaId!.Value];
                    return Math.Max(oN, oE) < Math.Min(dN, dE);
                })
                .Any();

            if (hayConflicto)
                throw new InvalidOperationException("Asiento no disponible en el tramo seleccionado.");

            // -------- Crear reserva (bloqueo por clic) --------
            var nuevo = new Boleto
            {
                IdViaje = request.IdViaje,
                IdAsiento = request.IdAsiento,
                IdCliente = request.IdCliente ?? 0,          // tu entidad usa int no-nullable
                OrigenParadaId = request.OrigenParadaId,
                DestinoParadaId = request.DestinoParadaId,
                Precio = request.Precio ?? 0m,               // se fijará al confirmar
                Estado = "BLOQUEADO",
                FechaReservaUtc = ahoraLocal,
                FechaCompra = ahoraLocal                     // compatibilidad
            };

            await _boletoRepo.AddAsync(nuevo);

            return new Response<int>(
                nuevo.IdBoleto,
                $"Asiento reservado. Expira en {request.ReservaTtlMinutos} min (reservas dejan de bloquear a T–2h)."
            );
        }
    }
}
