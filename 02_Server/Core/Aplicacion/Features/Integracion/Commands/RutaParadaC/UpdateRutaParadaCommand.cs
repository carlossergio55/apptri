using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Integracion;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.RutaParadaC
{
    public class UpdateRutaParadaCommand : IRequest<Response<int>>
    {
        public int IdRutaParada { get; set; } 
        public int Orden { get; set; }
    }

    public class UpdateRutaParadaCommandHandler : IRequestHandler<UpdateRutaParadaCommand, Response<int>>
    {
        private readonly IRepositoryAsync<RutaParada> _repo;

        public UpdateRutaParadaCommandHandler(IRepositoryAsync<RutaParada> repo) => _repo = repo;

        public async Task<Response<int>> Handle(UpdateRutaParadaCommand request, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(request.IdRutaParada);

            if (entity == null)
                throw new KeyNotFoundException("Ruta-Parada no encontrada.");

            entity.Orden = request.Orden;

            await _repo.UpdateAsync(entity);

            return new Response<int>(request.IdRutaParada);
        }
    }
}
