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

namespace Aplicacion.Features.Integracion.Commands
{

    public class DeleteUsuarioCommand : IRequest<Response<int>>
    {
        public int IdUsuario { get; set; }
    }

    public class DeleteUsuarioCommandHandler : IRequestHandler<DeleteUsuarioCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Usuario> _repositoryAsync;
        public DeleteUsuarioCommandHandler(IRepositoryAsync<Usuario> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<int>> Handle(DeleteUsuarioCommand request, CancellationToken cancellationToken)
        {
            var _Usuario = await _repositoryAsync.GetByIdAsync(request.IdUsuario);
            if (_Usuario == null)
            {
                throw new KeyNotFoundException("Registro no encontrado con el id");
            }
            else
            {
                await _repositoryAsync.DeleteAsync(_Usuario);
                return new Response<int>(_Usuario.IdUsuario);
            }
        }

       
    }
}
