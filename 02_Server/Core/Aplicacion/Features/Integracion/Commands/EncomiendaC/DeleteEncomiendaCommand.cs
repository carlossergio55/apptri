using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Integracion;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.EncomiendaC
{
    public class DeleteEncomiendaCommand : IRequest<Response<int>>
    {
        public int IdEncomienda { get; set; }
    }

    public class DeleteEncomiendaCommandHandler : IRequestHandler<DeleteEncomiendaCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Encomienda> _repositoryAsync;

        public DeleteEncomiendaCommandHandler(IRepositoryAsync<Encomienda> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<int>> Handle(DeleteEncomiendaCommand request, CancellationToken cancellationToken)
        {
            var encomienda = await _repositoryAsync.GetByIdAsync(request.IdEncomienda);
            if (encomienda == null)
                throw new KeyNotFoundException("Registro no encontrado.");

            // Regla mínima: no borrar si está pagada
            if (encomienda.Pagado)
                throw new System.Exception("No se puede eliminar una encomienda pagada. Use anulación/reembolso.");

            await _repositoryAsync.DeleteAsync(encomienda);
            return new Response<int>(encomienda.IdEncomienda);
        }
    }
}
