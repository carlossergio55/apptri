using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Integracion;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.GuiaCargaC
{
    public class DeleteGuiaCargaCommand : IRequest<Response<int>>
    {
        public int IdGuiaCarga { get; set; }
    }

    public class DeleteGuiaCargaCommandHandler : IRequestHandler<DeleteGuiaCargaCommand, Response<int>>
    {
        private readonly IRepositoryAsync<GuiaCarga> _repo;

        public DeleteGuiaCargaCommandHandler(IRepositoryAsync<GuiaCarga> repo) => _repo = repo;

        public async Task<Response<int>> Handle(DeleteGuiaCargaCommand request, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(request.IdGuiaCarga);
            if (entity == null) throw new KeyNotFoundException("Registro no encontrado.");

            await _repo.DeleteAsync(entity);
            return new Response<int>(entity.IdGuiaCarga);
        }
    }
}
