using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Integracion;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.ParadaC
{
    public class DeleteParadaCommand : IRequest<Response<int>>
    {
        public int IdParada { get; set; }
    }

    public class DeleteParadaCommandHandler : IRequestHandler<DeleteParadaCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Parada> _repo;

        public DeleteParadaCommandHandler(IRepositoryAsync<Parada> repo) => _repo = repo;

        public async Task<Response<int>> Handle(DeleteParadaCommand request, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(request.IdParada);
            if (entity == null) throw new KeyNotFoundException("Registro no encontrado.");

            await _repo.DeleteAsync(entity);
            return new Response<int>(entity.IdParada);
        }
    }
}
