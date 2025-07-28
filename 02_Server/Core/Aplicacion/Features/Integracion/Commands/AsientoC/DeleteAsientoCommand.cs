using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Integracion;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.AsientoC
{
    public class DeleteAsientoCommand : IRequest<Response<int>>
    {
        public int IdAsiento { get; set; }

     
    }

    public class DeleteAsientoCommandHandler : IRequestHandler<DeleteAsientoCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Asiento> _repositoryAsync;

        public DeleteAsientoCommandHandler(IRepositoryAsync<Asiento> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<int>> Handle(DeleteAsientoCommand request, CancellationToken cancellationToken)
        {
            var asiento = await _repositoryAsync.GetByIdAsync(request.IdAsiento);
            if (asiento == null)
                throw new KeyNotFoundException("Registro no encontrado");

            await _repositoryAsync.DeleteAsync(asiento);
            return new Response<int>(asiento.IdAsiento);
        }
    }
}
