using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Integracion;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.ViajeC
{
    public class DeleteViajeCommand : IRequest<Response<int>>
    {
        public int IdViaje { get; set; }

       
    }

    public class DeleteViajeCommandHandler : IRequestHandler<DeleteViajeCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Viaje> _repositoryAsync;

        public DeleteViajeCommandHandler(IRepositoryAsync<Viaje> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<int>> Handle(DeleteViajeCommand request, CancellationToken cancellationToken)
        {
            var viaje = await _repositoryAsync.GetByIdAsync(request.IdViaje);
            if (viaje == null)
                throw new KeyNotFoundException("Registro no encontrado");

            await _repositoryAsync.DeleteAsync(viaje);
            return new Response<int>(viaje.IdViaje);
        }
    }
}
