using Aplicacion.DTOs.Integracion;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.PagoC
{
    public class CreatePagoCommand : IRequest<Response<int>>
    {
        public PagoDto Pago { get; set; } = default!;
    }

    public class CreatePagoCommandHandler : IRequestHandler<CreatePagoCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Pago> _repo;
        private readonly IApplicationDbContext _ctx;
        private readonly IMapper _mapper;

        public CreatePagoCommandHandler(
            IRepositoryAsync<Pago> repo,
            IApplicationDbContext ctx,
            IMapper mapper)
        {
            _repo = repo;
            _ctx = ctx;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreatePagoCommand request, CancellationToken ct)
        {
            if (request.Pago is null)
                throw new ValidationException("Datos de pago requeridos.");

            // --- Normalizamos TipoPago (Boleto/Encomienda) ---
            var tipoRaw = request.Pago.TipoPago ?? string.Empty;
            var tipo = tipoRaw.Trim().ToLowerInvariant();

            bool esBoleto = tipo == "boleto";
            bool esEncomienda = tipo == "encomienda";
            if (!esBoleto && !esEncomienda)
                throw new ValidationException($"TipoPago '{tipoRaw}' no es válido. Use 'Boleto' o 'Encomienda'.");

            // --- Validamos existencia de la referencia (IdReferencia) ---
            var refId = request.Pago.IdReferencia;
            bool refValida = esBoleto
                ? await _ctx.Boleto.AnyAsync(b => b.IdBoleto == refId, ct)
                : await _ctx.Encomienda.AnyAsync(e => e.IdEncomienda == refId, ct);

            if (!refValida)
                throw new ValidationException($"La referencia con ID {refId} no existe para el tipo de pago '{tipoRaw}'.");

            // --- Determinamos monto desde la referencia ---
            decimal monto = esBoleto
                ? await _ctx.Boleto.Where(b => b.IdBoleto == refId).Select(b => b.Precio).FirstAsync(ct)
                : await _ctx.Encomienda.Where(e => e.IdEncomienda == refId).Select(e => e.Precio).FirstAsync(ct);

            // --- Normalizamos y validamos Metodo/Referencia ---
            var metodo = (request.Pago.Metodo ?? "").Trim().ToUpperInvariant();
            if (metodo is not ("EFECTIVO" or "QR" or "TRANSFERENCIA"))
                throw new ValidationException("Método de pago inválido. Use EFECTIVO, QR o TRANSFERENCIA.");

            string? referenciaNorm = null;
            if (metodo == "EFECTIVO")
            {
                // Para efectivo, no se usa referencia
                referenciaNorm = null;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(request.Pago.Referencia))
                    throw new ValidationException("La referencia es obligatoria para pagos con QR/TRANSFERENCIA.");

                referenciaNorm = request.Pago.Referencia!.Trim();

               
                var refLower = referenciaNorm.ToLower();
                var yaExiste = await _ctx.Pago.AnyAsync(p =>
                        p.Metodo.ToLower() == metodo.ToLower()
                        && p.Referencia != null
                        && p.Referencia.ToLower().Trim() == refLower,
                    ct);

                if (yaExiste)
                    throw new ValidationException("La referencia indicada ya fue utilizada en otro pago.");
            }

            // Escribimos de vuelta los valores normalizados al DTO
            request.Pago.Metodo = metodo;
            request.Pago.Referencia = referenciaNorm;
            request.Pago.Monto = monto;

            // --- Persistimos ---
            var pago = _mapper.Map<Pago>(request.Pago);
            var saved = await _repo.AddAsync(pago);
            return new Response<int>(saved.IdPago);
        }
    }
}
