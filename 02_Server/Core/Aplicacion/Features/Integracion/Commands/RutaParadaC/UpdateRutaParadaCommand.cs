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
    public class UpdateRutaParadaCommand : IRequest<Response<int>>
    {
        public int IdRuta { get; set; }
        public int IdParada { get; set; }
        public int Orden { get; set; }
    }

    public class UpdateRutaParadaCommandHandler : IRequestHandler<UpdateRutaParadaCommand, Response<int>>
    {
        private readonly IRepositoryAsync<RutaParada> _repo;
        public UpdateRutaParadaCommandHandler(IRepositoryAsync<RutaParada> repo) => _repo = repo;

        public async Task<Response<int>> Handle(UpdateRutaParadaCommand request, CancellationToken ct)
        {
            var all = await _repo.ListAsync();
            var entity = all.FirstOrDefault(x => x.IdRuta == request.IdRuta && x.IdParada == request.IdParada);
            if (entity == null) throw new KeyNotFoundException("Registro no encontrado.");

            entity.Orden = request.Orden;

            await _repo.UpdateAsync(entity);
            return new Response<int>(1); 
        }
    }
}
