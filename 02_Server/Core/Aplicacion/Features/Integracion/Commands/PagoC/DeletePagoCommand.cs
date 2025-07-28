using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Integracion;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.PagoC
{
    public class DeletePagoCommand : IRequest<Response<int>>
    {
        public int IdPago { get; set; }

        
    }

    public class DeletePagoCommandHandler : IRequestHandler<DeletePagoCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Pago> _repositoryAsync;

        public DeletePagoCommandHandler(IRepositoryAsync<Pago> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<int>> Handle(DeletePagoCommand request, CancellationToken cancellationToken)
        {
            var pago = await _repositoryAsync.GetByIdAsync(request.IdPago);
            if (pago == null)
                throw new KeyNotFoundException("Registro no encontrado");

            await _repositoryAsync.DeleteAsync(pago);
            return new Response<int>(pago.IdPago);
        }
    }
}

