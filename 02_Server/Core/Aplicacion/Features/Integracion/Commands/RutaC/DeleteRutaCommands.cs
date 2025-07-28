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

namespace Aplicacion.Features.Integracion.Commands.RutaC
{

    public class DeleteRutaCommand : IRequest<Response<int>>
    {
        public int IdRuta { get; set; }
    }

    public class DeleteRutaCommandHandler : IRequestHandler<DeleteRutaCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Ruta> _repositoryAsync;
        public DeleteRutaCommandHandler(IRepositoryAsync<Ruta> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<int>> Handle(DeleteRutaCommand request, CancellationToken cancellationToken)
        {
            var _Ruta = await _repositoryAsync.GetByIdAsync(request.IdRuta);
            if (_Ruta == null)
            {
                throw new KeyNotFoundException("Registro no encontrado con el id");
            }
            else
            {
                await _repositoryAsync.DeleteAsync(_Ruta);
                return new Response<int>(_Ruta.IdRuta);
            }
        }

       
    }
}
