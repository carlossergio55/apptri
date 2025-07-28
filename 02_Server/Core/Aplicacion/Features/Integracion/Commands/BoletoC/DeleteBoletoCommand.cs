using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Integracion;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.BoletoC
{
    public class DeleteBoletoCommand : IRequest<Response<int>>
    {
        public int IdBoleto { get; set; }

      
    }

    public class DeleteBoletoCommandHandler : IRequestHandler<DeleteBoletoCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Boleto> _repositoryAsync;

        public DeleteBoletoCommandHandler(IRepositoryAsync<Boleto> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<int>> Handle(DeleteBoletoCommand request, CancellationToken cancellationToken)
        {
            var boleto = await _repositoryAsync.GetByIdAsync(request.IdBoleto);
            if (boleto == null)
                throw new KeyNotFoundException("Registro no encontrado");

            await _repositoryAsync.DeleteAsync(boleto);
            return new Response<int>(boleto.IdBoleto);
        }
    }
}
