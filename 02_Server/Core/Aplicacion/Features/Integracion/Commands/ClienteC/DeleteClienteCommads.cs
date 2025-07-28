using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Integracion;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.ClienteC
{

    public class DeleteClienteCommand : IRequest<Response<int>>
    {
        public int IdCliente { get; set; }
    }

    public class DeleteClienteCommandHandler : IRequestHandler<DeleteClienteCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Cliente> _repositoryAsync;
        public DeleteClienteCommandHandler(IRepositoryAsync<Cliente> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<int>> Handle(DeleteClienteCommand request, CancellationToken cancellationToken)
        {
            var _Cliente = await _repositoryAsync.GetByIdAsync(request.IdCliente);
            if (_Cliente == null)
            {
                throw new KeyNotFoundException("Registro no encontrado con el id");
            }
            else
            {
                await _repositoryAsync.DeleteAsync(_Cliente);
                return new Response<int>(_Cliente.IdCliente);
            }
        }

       
    }
}
