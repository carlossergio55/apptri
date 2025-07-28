using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.BoletoC
{
    public class UpdateBoletoCommand : IRequest<Response<int>>
    {
        public int IdBoleto { get; set; }
        public decimal Precio { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCompra { get; set; }
        public int IdViaje { get; set; }
        public int IdCliente { get; set; }
        public int IdAsiento { get; set; }
        public int? IdPago { get; set; }
    }

    public class UpdateBoletoCommandHandler : IRequestHandler<UpdateBoletoCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Boleto> _repositoryAsync;
        private readonly IMapper _mapper;

        public UpdateBoletoCommandHandler(IRepositoryAsync<Boleto> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(UpdateBoletoCommand request, CancellationToken cancellationToken)
        {
            var boleto = await _repositoryAsync.GetByIdAsync(request.IdBoleto);
            if (boleto == null)
                throw new KeyNotFoundException("Registro no encontrado");

            boleto.Precio = request.Precio;
            boleto.Estado = request.Estado;
            boleto.FechaCompra = request.FechaCompra;
            boleto.IdViaje = request.IdViaje;
            boleto.IdCliente = request.IdCliente;
            boleto.IdAsiento = request.IdAsiento;
            boleto.IdPago = request.IdPago;

            await _repositoryAsync.UpdateAsync(boleto);
            return new Response<int>(boleto.IdBoleto);
        }
    }
}
