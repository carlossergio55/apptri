using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Integracion;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.ViajeC
{
    public class UpdateViajeCommand : IRequest<Response<int>>
    {
        public int IdViaje { get; set; }
        public System.DateTime Fecha { get; set; }
        public string HoraSalida { get; set; } = string.Empty; // "HH:mm" o "HH:mm:ss"
        public string Estado { get; set; } = string.Empty;

        public int IdRuta { get; set; }
        public int IdChofer { get; set; }
        public int IdBus { get; set; }

        // 👇 Nuevos campos
        public string? Direccion { get; set; }           // "IDA" | "VUELTA"
        public int? DesdeParadaId { get; set; }
        public int? HastaParadaId { get; set; }
    }

    public class UpdateViajeCommandHandler : IRequestHandler<UpdateViajeCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Viaje> _repositoryAsync;

        public UpdateViajeCommandHandler(IRepositoryAsync<Viaje> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<int>> Handle(UpdateViajeCommand request, CancellationToken cancellationToken)
        {
            var viaje = await _repositoryAsync.GetByIdAsync(request.IdViaje);
            if (viaje == null)
                throw new KeyNotFoundException("Registro no encontrado.");

            viaje.Fecha = request.Fecha;

            // Parseo simple de hora (string -> TimeSpan)
            if (!System.TimeSpan.TryParse(request.HoraSalida, out var ts))
                throw new System.Exception("Formato inválido para HoraSalida. Usa HH:mm o HH:mm:ss.");
            viaje.HoraSalida = ts;

            viaje.Estado = request.Estado;
            viaje.IdRuta = request.IdRuta;
            viaje.IdChofer = request.IdChofer;
            viaje.IdBus = request.IdBus;

            // Nuevos campos
            viaje.Direccion = string.IsNullOrWhiteSpace(request.Direccion)
                                  ? "IDA"
                                  : request.Direccion.Trim().ToUpperInvariant();
            viaje.DesdeParadaId = request.DesdeParadaId;
            viaje.HastaParadaId = request.HastaParadaId;

            await _repositoryAsync.UpdateAsync(viaje);
            return new Response<int>(viaje.IdViaje);
        }
    }
}
