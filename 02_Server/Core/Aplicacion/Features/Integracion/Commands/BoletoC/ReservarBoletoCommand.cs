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

            var ahora = DateTime.Now;
            var salidaLocal = viaje.Fecha.Date + viaje.HoraSalida;   // HoraSalida = TimeSpan
            var venceReservasLocal = salidaLocal.AddHours(-2);        // corte T–2h

            var boletosAll = await _boletoRepo.ListAsync();

            var conflictivo = boletosAll
                .Where(b => b.IdViaje == request.IdViaje && b.IdAsiento == request.IdAsiento)
                .Where(b => b.OrigenParadaId.HasValue && b.DestinoParadaId.HasValue)
                .Where(b =>
                {
                    var estado = (b.Estado ?? "").ToUpperInvariant();

                    // Si tu entidad Boleto.FechaCompra es NO-nullable, usa directo:
                    var fechaCompra = b.FechaCompra ?? ahora;

                    var minutosBloqueo = (ahora - fechaCompra).TotalMinutes;

                    var bloqueaPorEstado =
                           estado == "PAGADO"
                        || (estado == "BLOQUEADO"
                            && ahora <= venceReservasLocal
                            && minutosBloqueo <= request.ReservaTtlMinutos);

                    if (!bloqueaPorEstado) return false;

                    var oE = ordenPorParada[b.OrigenParadaId!.Value];
                    var dE = ordenPorParada[b.DestinoParadaId!.Value];

                    // Solape de tramos
                    return Math.Max(oN, oE) < Math.Min(dN, dE);
                })
                .Any();

            if (conflictivo)
                throw new InvalidOperationException("Asiento no disponible en el tramo seleccionado.");

            var nuevo = new Boleto
            {
                IdViaje = request.IdViaje,
                IdAsiento = request.IdAsiento,
                IdCliente = request.IdCliente,
                OrigenParadaId = request.OrigenParadaId,
                DestinoParadaId = request.DestinoParadaId,
                Precio = request.Precio,
                Estado = "BLOQUEADO",
                FechaCompra = ahora
            };

            await _boletoRepo.AddAsync(nuevo);

            return new Response<int>(nuevo.IdBoleto, "Asiento reservado.");
        }
    }
}
