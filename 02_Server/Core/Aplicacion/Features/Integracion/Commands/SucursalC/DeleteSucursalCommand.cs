using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Integracion;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.SucursalC
{
    public class DeleteSucursalCommand : IRequest<Response<int>>
    {
        public int IdSucursal { get; set; }
    }

    public class DeleteSucursalCommandHandler : IRequestHandler<DeleteSucursalCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Sucursal> _repositoryAsync;

        public DeleteSucursalCommandHandler(IRepositoryAsync<Sucursal> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<int>> Handle(DeleteSucursalCommand request, CancellationToken cancellationToken)
        {
            var sucursal = await _repositoryAsync.GetByIdAsync(request.IdSucursal);

            if (sucursal == null)
            {
                throw new KeyNotFoundException("Sucursal no encontrada con el id proporcionado.");
            }

            await _repositoryAsync.DeleteAsync(sucursal);

            return new Response<int>(sucursal.IdSucursal);
        }
    }
}
