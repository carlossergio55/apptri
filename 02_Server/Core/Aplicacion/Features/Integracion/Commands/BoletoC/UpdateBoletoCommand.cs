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
    public class UpdateBoletoCommand : IRequest<Response<int>>
    {
        public int IdBoleto { get; set; }

        // Editables “administrativos”
        public decimal? Precio { get; set; }
        public int? IdCliente { get; set; }
        public int? IdPago { get; set; }          // solo si ya fue pagado y se quiere vincular

        // Ajuste de tramo (NO mover asiento/viaje aquí)
        public int? OrigenParadaId { get; set; }
        public int? DestinoParadaId { get; set; }

        // Cambio de estado (opcional): BLOQUEADO | PAGADO | ANULADO
        public string? Estado { get; set; }
    }

    public class UpdateBoletoCommandHandler : IRequestHandler<UpdateBoletoCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Boleto> _repo;

        public UpdateBoletoCommandHandler(IRepositoryAsync<Boleto> repo)
        {
            _repo = repo;
        }

        public async Task<Response<int>> Handle(UpdateBoletoCommand request, CancellationToken ct)
        {
            var b = await _repo.GetByIdAsync(request.IdBoleto);
            if (b is null) throw new KeyNotFoundException("Registro no encontrado.");

            // --- Campos editables simples ---
            if (request.Precio.HasValue) b.Precio = request.Precio.Value;
            if (request.IdCliente.HasValue) b.IdCliente = request.IdCliente.Value;
            if (request.OrigenParadaId.HasValue) b.OrigenParadaId = request.OrigenParadaId;
            if (request.DestinoParadaId.HasValue) b.DestinoParadaId = request.DestinoParadaId;

            // --- Cambio de estado controlado ---
            if (!string.IsNullOrWhiteSpace(request.Estado))
            {
                var nuevo = request.Estado.Trim().ToUpperInvariant();

                switch (nuevo)
                {
                    case "BLOQUEADO":
                        b.Estado = "BLOQUEADO";
                        b.FechaReservaUtc = DateTime.UtcNow;
                        b.IdPago = null; // por seguridad
                        break;

                    case "PAGADO":
                        b.Estado = "PAGADO";
                        b.FechaConfirmacionUtc = DateTime.UtcNow;
                        // si deseas, registra timbre local de compra (solo al pagar)
                        b.FechaCompra ??= DateTime.Now;
                        if (request.IdPago.HasValue) b.IdPago = request.IdPago.Value;
                        break;

                    case "ANULADO":
                        b.Estado = "ANULADO";
                        b.IdPago = null; // sin pago asociado
                        break;

                    default:
                        throw new InvalidOperationException("Estado inválido.");
                }
            }
            else
            {
                // si solo quieren asociar/desasociar pago cuando ya está pagado
                if (request.IdPago.HasValue && string.Equals(b.Estado, "PAGADO", StringComparison.OrdinalIgnoreCase))
                    b.IdPago = request.IdPago.Value;
            }

 
            await _repo.UpdateAsync(b);
            return new Response<int>(b.IdBoleto, "Boleto actualizado.");
        }
    }
}
