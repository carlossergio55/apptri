using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Integracion;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.RutaParadaC
{
    public class DeleteRutaParadaCommand : IRequest<Response<int>>
    {
        public int IdRuta { get; set; }
        public int IdParada { get; set; }
    }

    public class DeleteRutaParadaCommandHandler : IRequestHandler<DeleteRutaParadaCommand, Response<int>>
    {
        private readonly IRepositoryAsync<RutaParada> _repo;

        public DeleteRutaParadaCommandHandler(IRepositoryAsync<RutaParada> repo) => _repo = repo;

        public async Task<Response<int>> Handle(DeleteRutaParadaCommand request, CancellationToken ct)
        {
            // Si tu IRepositoryAsync soporta GetByIdAsync con claves compuestas, úsalo.
            // Aquí lo resolvemos en memoria por simplicidad:
            var all = await _repo.ListAsync();
            var entity = all.FirstOrDefault(x => x.IdRuta == request.IdRuta && x.IdParada == request.IdParada);

            if (entity == null) throw new KeyNotFoundException("Registro no encontrado.");

            await _repo.DeleteAsync(entity);
            return new Response<int>(1); // PK compuesta
        }
    }
}
