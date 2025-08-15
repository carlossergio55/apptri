using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Integracion;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.GuiaCargaC
{
    public class UpdateGuiaCargaCommand : IRequest<Response<int>>
    {
        public int IdGuiaCarga { get; set; }
        public string Codigo { get; set; } = string.Empty;
    }

    public class UpdateGuiaCargaCommandHandler : IRequestHandler<UpdateGuiaCargaCommand, Response<int>>
    {
        private readonly IRepositoryAsync<GuiaCarga> _repo;
        public UpdateGuiaCargaCommandHandler(IRepositoryAsync<GuiaCarga> repo) => _repo = repo;

        public async Task<Response<int>> Handle(UpdateGuiaCargaCommand request, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(request.IdGuiaCarga);
            if (entity == null) throw new KeyNotFoundException("Registro no encontrado.");

            entity.Codigo = request.Codigo;

            await _repo.UpdateAsync(entity);
            return new Response<int>(entity.IdGuiaCarga);
        }
    }
}
