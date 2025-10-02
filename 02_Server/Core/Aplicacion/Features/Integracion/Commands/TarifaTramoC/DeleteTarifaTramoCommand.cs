using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Integracion;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.TarifaTramoC
{
    // Borra por PK única
    public class DeleteTarifaTramoCommand : IRequest<Response<int>>
    {
        public int IdTarifaTramo { get; set; }
    }

    public class DeleteTarifaTramoCommandHandler
        : IRequestHandler<DeleteTarifaTramoCommand, Response<int>>
    {
        private readonly IRepositoryAsync<TarifaTramo> _repo;
        public DeleteTarifaTramoCommandHandler(IRepositoryAsync<TarifaTramo> repo) => _repo = repo;

        public async Task<Response<int>> Handle(DeleteTarifaTramoCommand request, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(request.IdTarifaTramo);
            if (entity is null)
                throw new KeyNotFoundException($"TarifaTramo {request.IdTarifaTramo} no encontrada.");

            await _repo.DeleteAsync(entity);
            return new Response<int>(request.IdTarifaTramo); // devolvemos el Id eliminado
        }
    }
}
