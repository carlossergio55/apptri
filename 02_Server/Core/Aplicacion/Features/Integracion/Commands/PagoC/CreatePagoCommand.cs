using Aplicacion.DTOs.Integracion;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace Aplicacion.Features.Integracion.Commands.PagoC
{

    public class CreatePagoCommand : IRequest<Response<int>>
    {
        public PagoDto Pago { get; set; }
    }

    public class CreatePagoCommandHandler : IRequestHandler<CreatePagoCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Pago> _repositoryAsync;
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
       

        public CreatePagoCommandHandler(
            IRepositoryAsync<Pago> repositoryAsync,
            IApplicationDbContext context,
            IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _context = context;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreatePagoCommand request, CancellationToken cancellationToken)
        {

            // Validar existencia de la referencia según TipoPago
            var tipo = request.Pago.TipoPago;
            var referencia = request.Pago.IdReferencia;

            var referenciaValida = tipo switch
            {
                "Boleto" => await _context.Boleto.AnyAsync(b => b.IdBoleto == referencia, cancellationToken),
                "Encomienda" => await _context.Encomienda.AnyAsync(e => e.IdEncomienda == referencia, cancellationToken),
                _ => false
            };

            if (!referenciaValida)
            {
                throw new KeyNotFoundException($"La referencia con ID {referencia} no existe para el tipo de pago '{tipo}'.");
            }

            // Obtener monto automáticamente
            decimal montoCalculado = tipo switch
            {
                "Boleto" => await _context.Boleto
                                    .Where(b => b.IdBoleto == referencia)
                                    .Select(b => b.Precio)
                                    .FirstOrDefaultAsync(cancellationToken),

                "Encomienda" => await _context.Encomienda
                                    .Where(e => e.IdEncomienda == referencia)
                                    .Select(e => e.Precio)
                                    .FirstOrDefaultAsync(cancellationToken),

                _ => 0
            };

            // Sobrescribir el monto
            request.Pago.Monto = montoCalculado;


            var nuevoRegistro = _mapper.Map<Pago>(request.Pago);
            var data = await _repositoryAsync.AddAsync(nuevoRegistro);
            return new Response<int>(data.IdPago);
        }

     
    }

   
}
