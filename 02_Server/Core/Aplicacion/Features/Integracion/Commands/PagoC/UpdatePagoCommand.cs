using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.PagoC
{
    public class UpdatePagoCommand : IRequest<Response<int>>
    {
        public int IdPago { get; set; }
        public string TipoPago { get; set; }
        public int IdReferencia { get; set; }
        public decimal Monto { get; set; }
        public string Metodo { get; set; }
        public DateTime FechaPago { get; set; }
        public int IdUsuario { get; set; }
        public int IdCliente { get; set; }
    }

    public class UpdatePagoCommandHandler : IRequestHandler<UpdatePagoCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Pago> _repositoryAsync;
        private readonly IMapper _mapper;

        public UpdatePagoCommandHandler(IRepositoryAsync<Pago> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(UpdatePagoCommand request, CancellationToken cancellationToken)
        {
            var pago = await _repositoryAsync.GetByIdAsync(request.IdPago);
            if (pago == null)
                throw new KeyNotFoundException("Registro no encontrado");

            pago.TipoPago = request.TipoPago;
            pago.IdReferencia = request.IdReferencia;
            pago.Monto = request.Monto;
            pago.Metodo = request.Metodo;
            pago.FechaPago = request.FechaPago;
            pago.IdUsuario = request.IdUsuario;
            pago.IdCliente = request.IdCliente;

            await _repositoryAsync.UpdateAsync(pago);
            return new Response<int>(pago.IdPago);
        }
    }
}

