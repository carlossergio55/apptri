using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Integracion;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.HorarioC
{
    public class DeleteHorarioCommand : IRequest<Response<int>>
    {
        public int IdHorario { get; set; }
    }

    public class DeleteHorarioCommandHandler : IRequestHandler<DeleteHorarioCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Horario> _repositoryAsync;

        public DeleteHorarioCommandHandler(IRepositoryAsync<Horario> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<int>> Handle(DeleteHorarioCommand request, CancellationToken cancellationToken)
        {
            var horario = await _repositoryAsync.GetByIdAsync(request.IdHorario);
            if (horario == null)
                throw new KeyNotFoundException("Registro no encontrado.");

            await _repositoryAsync.DeleteAsync(horario);
            return new Response<int>(horario.IdHorario);
        }
    }
}
