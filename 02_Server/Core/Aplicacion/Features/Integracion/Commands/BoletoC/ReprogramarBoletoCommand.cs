using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Aplicacion.Wrappers;
using Aplicacion.Interfaces;
using Dominio.Entities.Integracion;
using System.Collections.Generic;

namespace Aplicacion.Features.Integracion.Commands.BoletoC
{
    public class ReprogramarBoletoCommand : IRequest<Response<int>>
    {
        public int IdBoleto { get; set; }
        public int IdViajeDestino { get; set; }
        public int IdAsientoDestino { get; set; }
        public int? OrigenParadaId { get; set; }
        public int? DestinoParadaId { get; set; }
        public int? IdUsuario { get; set; }
        public string? Motivo { get; set; }

        public int ReservaTtlMinutos { get; set; } = 10;
    }

    public class ReprogramarBoletoCommandHandler : IRequestHandler<ReprogramarBoletoCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Boleto> _boletoRepo;
        private readonly IRepositoryAsync<Viaje> _viajeRepo;
        private readonly IRepositoryAsync<RutaParada> _rutaParadaRepo;
        private readonly IRepositoryAsync<Asiento> _asientoRepo;

        public ReprogramarBoletoCommandHandler(
            IRepositoryAsync<Boleto> boletoRepo,
            IRepositoryAsync<Viaje> viajeRepo,
            IRepositoryAsync<RutaParada> rutaParadaRepo,
            IRepositoryAsync<Asiento> asientoRepo)
        {
            _boletoRepo = boletoRepo;
            _viajeRepo = viajeRepo;
            _rutaParadaRepo = rutaParadaRepo;
            _asientoRepo = asientoRepo;
        }

        public async Task<Response<int>> Handle(ReprogramarBoletoCommand request, CancellationToken ct)
        {
            var boleto = await _boletoRepo.GetByIdAsync(request.IdBoleto);
            if (boleto is null) throw new KeyNotFoundException("Boleto no encontrado.");

            if (string.Equals(boleto.Estado, "ANULADO", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("No se puede reprogramar un boleto ANULADO.");

            // Viaje/Asiento destino
            var viajeDest = await _viajeRepo.GetByIdAsync(request.IdViajeDestino);
            if (viajeDest is null) throw new KeyNotFoundException("Viaje destino no encontrado.");

            var asientoDest = await _asientoRepo.GetByIdAsync(request.IdAsientoDestino);
            if (asientoDest is null) throw new KeyNotFoundException("Asiento destino no encontrado.");
            if (asientoDest.IdBus != viajeDest.IdBus)
                throw new InvalidOperationException("El asiento destino no pertenece al bus del viaje destino.");

            // Tramo destino (si no envían, usar el del boleto)
            var origenId = request.OrigenParadaId ?? boleto.OrigenParadaId ?? 0;
            var destinoId = request.DestinoParadaId ?? boleto.DestinoParadaId ?? 0;
            if (origenId == 0 || destinoId == 0)
                throw new InvalidOperationException("El boleto no tiene tramo definido.");
            if (origenId == destinoId)
                throw new InvalidOperationException("Origen y destino no pueden ser iguales.");

            // Orden de paradas
            var rpsAll = await _rutaParadaRepo.ListAsync();
            var orden = rpsAll
                .Where(x => x.IdRuta == viajeDest.IdRuta)
                .OrderBy(x => x.Orden)
                .ToDictionary(x => x.IdParada, x => x.Orden);

            if (!orden.ContainsKey(origenId) || !orden.ContainsKey(destinoId))
                throw new InvalidOperationException("Paradas del tramo no pertenecen a la ruta del viaje destino.");

            var oN = orden[origenId];
            var dN = orden[destinoId];
            if (oN >= dN)
                throw new InvalidOperationException("El orden de paradas es inválido (origen debe ser antes que destino).");

            // Validar solape por tramo en el viaje/asiento destino
            var ahoraUtc = DateTime.UtcNow;

            var boletosAll = await _boletoRepo.ListAsync();
            var conflictivos = boletosAll
                .Where(b => b.IdViaje == request.IdViajeDestino &&
                            b.IdAsiento == request.IdAsientoDestino &&
                            b.IdBoleto != boleto.IdBoleto)
                .Where(b =>
                {
                    var esPagado = string.Equals(b.Estado, "PAGADO", StringComparison.OrdinalIgnoreCase);

                    // bloqueado vigente dentro del TTL
                    var fechaBase = b.FechaReservaUtc
                                    ?? b.FechaCompra          // si aún usas FechaCompra
                                    ?? ahoraUtc;              // fallback => 0 min
                    var minutosBloqueo = (ahoraUtc - fechaBase).TotalMinutes;
                    var bloqueadoVigente = string.Equals(b.Estado, "BLOQUEADO", StringComparison.OrdinalIgnoreCase)
                                           && minutosBloqueo <= request.ReservaTtlMinutos;

                    return esPagado || bloqueadoVigente;
                })
                .Where(b => b.OrigenParadaId.HasValue && b.DestinoParadaId.HasValue)
                .Where(b =>
                {
                    var oE = orden[b.OrigenParadaId!.Value];
                    var dE = orden[b.DestinoParadaId!.Value];
                    // solape de tramos: [oN,dN) vs [oE,dE)
                    return Math.Max(oN, oE) < Math.Min(dN, dE);
                })
                .ToList();

            if (conflictivos.Any())
                throw new InvalidOperationException("El asiento destino no está disponible en el tramo.");

            // Actualizar boleto
            boleto.IdViaje = request.IdViajeDestino;
            boleto.IdAsiento = request.IdAsientoDestino;
            boleto.OrigenParadaId = origenId;
            boleto.DestinoParadaId = destinoId;

            await _boletoRepo.UpdateAsync(boleto);

            return new Response<int>(boleto.IdBoleto, "Boleto reprogramado.");
        }
    }
}
