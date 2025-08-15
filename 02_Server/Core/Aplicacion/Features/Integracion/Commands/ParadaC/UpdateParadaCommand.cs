using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Integracion;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.ParadaC
{
    public class UpdateParadaCommand : IRequest<Response<int>>
    {
        public int IdParada { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    public class UpdateParadaCommandHandler : IRequestHandler<UpdateParadaCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Parada> _repo;
        public UpdateParadaCommandHandler(IRepositoryAsync<Parada> repo) => _repo = repo;

        public async Task<Response<int>> Handle(UpdateParadaCommand request, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(request.IdParada);
            if (entity == null) throw new KeyNotFoundException("Registro no encontrado.");

            entity.Nombre = request.Nombre;

            await _repo.UpdateAsync(entity);
            return new Response<int>(entity.IdParada);
        }
    }
}
