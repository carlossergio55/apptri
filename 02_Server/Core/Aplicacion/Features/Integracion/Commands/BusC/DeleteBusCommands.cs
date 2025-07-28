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

namespace Aplicacion.Features.Integracion.Commands.BusC
{

    public class DeleteBusCommand : IRequest<Response<int>>
    {
        public int IdBus { get; set; }
    }

    public class DeleteBusCommandHandler : IRequestHandler<DeleteBusCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Bus> _repositoryAsync;
        public DeleteBusCommandHandler(IRepositoryAsync<Bus> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<int>> Handle(DeleteBusCommand request, CancellationToken cancellationToken)
        {
            var _Bus = await _repositoryAsync.GetByIdAsync(request.IdBus);
            if (_Bus == null)
            {
                throw new KeyNotFoundException("Registro no encontrado con el id");
            }
            else
            {
                await _repositoryAsync.DeleteAsync(_Bus);
                return new Response<int>(_Bus.IdBus);
            }
        }

        
    }
}
