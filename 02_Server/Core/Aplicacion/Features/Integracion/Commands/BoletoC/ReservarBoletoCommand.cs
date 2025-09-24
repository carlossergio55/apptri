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
        public int IdCliente { get; set; }
        public int OrigenParadaId { get; set; }
        public int DestinoParadaId { get; set; }
        public decimal Precio { get; set; }
        public int ReservaTtlMinutos { get; set; } = 10; // bloqueo por clic
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
            // -------- Validaciones básicas --------
            if (request.OrigenParadaId == request.DestinoParadaId)
                throw new InvalidOperationException("Origen y destino no pueden ser iguales.");
            if (request.Precio <= 0m)
                throw new InvalidOperationException("Precio inválido.");
            if (request.IdCliente <= 0)
                throw new InvalidOperationException("Cliente requerido.");

            var viaje = await _viajeRepo.GetByIdAsync(request.IdViaje);
            if (viaje is null) throw new KeyNotFoundException("Viaje no encontrado.");

            var asiento = await _asientoRepo.GetByIdAsync(request.IdAsiento);
            if (asiento is null) throw new KeyNotFoundException("Asiento no encontrado.");
            if (asiento.IdBus != viaje.IdBus)
                throw new InvalidOperationException("El asiento no pertenece al bus del viaje.");

            var cliente = await _clienteRepo.GetByIdAsync(request.IdCliente);
            if (cliente is null) throw new KeyNotFoundException("Cliente no encontrado.");

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

            // -------- Cronología LOCAL coherente --------
            var ahoraLocal = DateTime.Now;                         // usa Now en TODO el flujo
            var salidaLocal = viaje.Fecha.Date + viaje.HoraSalida;  // DateOnly + TimeSpan => DateTime local
            var dosHorasAntes = salidaLocal.AddHours(-2);

            // Si ya estamos dentro de la ventana T–2h, no aceptamos reservas
            if (ahoraLocal >= dosHorasAntes)
                throw new InvalidOperationException("No se permiten reservas a menos de 2 horas de la salida.");

            // -------- Conflicto de asiento por tramo --------
            var boletosAll = await _boletoRepo.ListAsync();

            bool hayConflicto = boletosAll
                .Where(b => b.IdViaje == request.IdViaje && b.IdAsiento == request.IdAsiento)
                .Where(b => b.OrigenParadaId.HasValue && b.DestinoParadaId.HasValue)
                .Where(b =>
                {
                    var estado = (b.Estado ?? "").ToUpperInvariant();

                    var fechaBase = b.FechaReservaUtc ?? b.FechaCompra ?? ahoraLocal;
                    var minutos = (ahoraLocal - fechaBase).TotalMinutes;

                    // BLOQUEADO solo bloquea si aún NO estamos en la ventana T–2h y su TTL no venció
                    var bloqueaPorEstado =
                           estado == "PAGADO"
                        || (estado == "BLOQUEADO"
                            && ahoraLocal < dosHorasAntes
                            && minutos <= request.ReservaTtlMinutos);

                    if (!bloqueaPorEstado) return false;

                    // solape de tramos [oN,dN) vs [oE,dE)
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
                IdCliente = request.IdCliente,
                OrigenParadaId = request.OrigenParadaId,
                DestinoParadaId = request.DestinoParadaId,
                Precio = request.Precio,
                Estado = "BLOQUEADO",

                FechaReservaUtc = ahoraLocal,
                // legacy (si aún lo usas en reportes):
                FechaCompra = ahoraLocal
            };

            await _boletoRepo.AddAsync(nuevo);

            return new Response<int>(
                nuevo.IdBoleto,
                $"Asiento reservado. Expira en {request.ReservaTtlMinutos} min o 2 h antes de la salida."
            );
        }
    }
}
