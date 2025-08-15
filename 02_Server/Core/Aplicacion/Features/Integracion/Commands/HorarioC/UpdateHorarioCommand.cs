using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Integracion;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.HorarioC
{
    public class UpdateHorarioCommand : IRequest<Response<int>>
    {
        public int IdHorario { get; set; }
        public System.TimeSpan HoraSalida { get; set; }
        public string DiaSemana { get; set; } = string.Empty;
        public int IdRuta { get; set; }

       
        public string? Direccion { get; set; }          // "IDA" | "VUELTA"
        public int? DesdeParadaId { get; set; }
        public int? HastaParadaId { get; set; }
    }

    public class UpdateHorarioCommandHandler : IRequestHandler<UpdateHorarioCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Horario> _repositoryAsync;

        public UpdateHorarioCommandHandler(IRepositoryAsync<Horario> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<int>> Handle(UpdateHorarioCommand request, CancellationToken cancellationToken)
        {
            var horario = await _repositoryAsync.GetByIdAsync(request.IdHorario);
            if (horario == null)
                throw new KeyNotFoundException("Registro no encontrado.");

            // Asignaciones directas (sin validaciones)
            horario.HoraSalida = request.HoraSalida;
            horario.DiaSemana = request.DiaSemana;
            horario.IdRuta = request.IdRuta;

            // Nuevos campos
            horario.Direccion = string.IsNullOrWhiteSpace(request.Direccion)
                                        ? "IDA"
                                        : request.Direccion.Trim().ToUpperInvariant();
            horario.DesdeParadaId = request.DesdeParadaId;
            horario.HastaParadaId = request.HastaParadaId;

            await _repositoryAsync.UpdateAsync(horario);
            return new Response<int>(horario.IdHorario);
        }
    }
}
