using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Integracion;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.RutaParadaC
{
    public class DeleteRutaParadaCommand : IRequest<Response<int>>
    {
        public int IdRutaParada { get; set; }
    }

    public class DeleteRutaParadaCommandHandler : IRequestHandler<DeleteRutaParadaCommand, Response<int>>
    {
        private readonly IRepositoryAsync<RutaParada> _repo;

        public DeleteRutaParadaCommandHandler(IRepositoryAsync<RutaParada> repo) => _repo = repo;

        public async Task<Response<int>> Handle(DeleteRutaParadaCommand request, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(request.IdRutaParada);

            if (entity == null)
                throw new KeyNotFoundException("Ruta-Parada no encontrada.");

            await _repo.DeleteAsync(entity);

            return new Response<int>(request.IdRutaParada);
        }
    }
}
