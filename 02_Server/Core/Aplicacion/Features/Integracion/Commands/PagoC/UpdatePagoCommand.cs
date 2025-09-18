using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.PagoC
{
    public class UpdatePagoCommand : IRequest<Response<int>>
    {
        public int IdPago { get; set; }

        // "Boleto" | "Encomienda"
        public string TipoPago { get; set; } = "";

        // Id del boleto o encomienda (según TipoPago)
        public int IdReferencia { get; set; }

        // Se recalcula desde la referencia (el valor entrante se ignora)
        public decimal Monto { get; set; }

        // "EFECTIVO" | "QR" | "TRANSFERENCIA"
        public string Metodo { get; set; } = "";

        public DateTime FechaPago { get; set; }
        public int IdUsuario { get; set; }
        public int IdCliente { get; set; }

        // NUEVO: número/folio del banco/QR (nullable para EFECTIVO)
        public string? Referencia { get; set; }
    }

    public class UpdatePagoCommandHandler : IRequestHandler<UpdatePagoCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Pago> _repositoryAsync;
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UpdatePagoCommandHandler(
            IRepositoryAsync<Pago> repositoryAsync,
            IApplicationDbContext context,
            IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _context = context;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(UpdatePagoCommand request, CancellationToken ct)
        {
            var pago = await _repositoryAsync.GetByIdAsync(request.IdPago);
            if (pago == null)
                throw new KeyNotFoundException("Registro no encontrado.");

            // --- Normalizamos TipoPago ---
            var tipoRaw = request.TipoPago ?? string.Empty;               // <-- CORREGIDO
            var tipo = tipoRaw.Trim().ToLowerInvariant();
            bool esBoleto = tipo == "boleto";
            bool esEncomienda = tipo == "encomienda";
            if (!esBoleto && !esEncomienda)
                throw new ValidationException($"TipoPago '{tipoRaw}' no es válido. Use 'Boleto' o 'Encomienda'.");

            // --- Validamos existencia de la entidad referenciada ---
            var refId = request.IdReferencia;
            bool refValida = esBoleto
                ? await _context.Boleto.AnyAsync(b => b.IdBoleto == refId, ct)
                : await _context.Encomienda.AnyAsync(e => e.IdEncomienda == refId, ct);
            if (!refValida)
                throw new ValidationException($"La referencia con ID {refId} no existe para el tipo de pago '{tipoRaw}'.");

            // --- Recalculamos monto desde la referencia ---
            decimal monto = esBoleto
                ? await _context.Boleto.Where(b => b.IdBoleto == refId).Select(b => b.Precio).FirstAsync(ct)
                : await _context.Encomienda.Where(e => e.IdEncomienda == refId).Select(e => e.Precio).FirstAsync(ct);

            // --- Normalizamos y validamos Método + Referencia ---
            var metodo = (request.Metodo ?? "").Trim().ToUpperInvariant();
            if (metodo is not ("EFECTIVO" or "QR" or "TRANSFERENCIA"))
                throw new ValidationException("Método de pago inválido. Use EFECTIVO, QR o TRANSFERENCIA.");

            string? referenciaNorm = null;
            if (metodo == "EFECTIVO")
            {
                referenciaNorm = null; // EFECTIVO no requiere referencia
            }
            else
            {
                if (string.IsNullOrWhiteSpace(request.Referencia))
                    throw new ValidationException("La referencia es obligatoria para pagos con QR/TRANSFERENCIA.");

                referenciaNorm = request.Referencia.Trim();
                var refLower = referenciaNorm.ToLower();

                // Dedupe (excluye este mismo pago)
                bool existeOtro = await _context.Pago.AnyAsync(p =>
                        p.IdPago != request.IdPago &&
                        p.Metodo.ToLower() == metodo.ToLower() &&
                        p.Referencia != null &&
                        p.Referencia.ToLower().Trim() == refLower, ct);

                if (existeOtro)
                    throw new ValidationException("La referencia indicada ya fue utilizada en otro pago.");
            }

            // --- Asignación de cambios normalizados ---
            pago.TipoPago = esBoleto ? "Boleto" : "Encomienda"; // casing prolijo
            pago.IdReferencia = refId;
            pago.Monto = monto;            // fuerza consistencia
            pago.Metodo = metodo;
            pago.Referencia = referenciaNorm;   // null para EFECTIVO
            pago.FechaPago = request.FechaPago;
            pago.IdUsuario = request.IdUsuario;
            pago.IdCliente = request.IdCliente;

            await _repositoryAsync.UpdateAsync(pago);
            return new Response<int>(pago.IdPago);
        }
    }
}
