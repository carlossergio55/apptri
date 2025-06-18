using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities;
using Dominio.Entities.Seguridad;
using MediatR;

namespace Aplicacion.Features.Clasificador.Commands
{

    public class DeleteGenClasificadorCommand : IRequest<Response<int>>
    {
        public int IdgenClasificador { get; set; }
    }

    public class DeleteGenClasificadorCommandHandler : IRequestHandler<DeleteGenClasificadorCommand, Response<int>>
    {
        private readonly IRepositoryAsync<GenClasificador> _repositoryAsync;
        public DeleteGenClasificadorCommandHandler(IRepositoryAsync<GenClasificador> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<int>> Handle(DeleteGenClasificadorCommand request, CancellationToken cancellationToken)
        {
            var _GenClasificador = await _repositoryAsync.GetByIdAsync(request.IdgenClasificador);
            if (_GenClasificador == null)
            {
                throw new KeyNotFoundException("Registro no encontrado con el id");
            }
            else
            {
                await _repositoryAsync.DeleteAsync(_GenClasificador);
                return new Response<int>(_GenClasificador.IdgenClasificador);
            }
        }

       
    }
}
