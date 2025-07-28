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

namespace Aplicacion.Features.Integracion.Commands.ChoferC
{

    public class DeleteChoferCommand : IRequest<Response<int>>
    {
        public int IdChofer { get; set; }
    }

    public class DeleteChoferCommandHandler : IRequestHandler<DeleteChoferCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Chofer> _repositoryAsync;
        public DeleteChoferCommandHandler(IRepositoryAsync<Chofer> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<int>> Handle(DeleteChoferCommand request, CancellationToken cancellationToken)
        {
            var _Chofer = await _repositoryAsync.GetByIdAsync(request.IdChofer);
            if (_Chofer == null)
            {
                throw new KeyNotFoundException("Registro no encontrado con el id");
            }
            else
            {
                await _repositoryAsync.DeleteAsync(_Chofer);
                return new Response<int>(_Chofer.IdChofer);
            }
        }

      
    }
}
