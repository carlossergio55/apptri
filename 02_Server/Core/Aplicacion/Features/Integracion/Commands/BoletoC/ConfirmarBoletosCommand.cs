using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

using Aplicacion.Wrappers;
using Aplicacion.Interfaces;                 
using Dominio.Entities.Integracion;      

namespace Aplicacion.Features.Integracion.Commands.BoletoC
{
    // ===== Request =====
    public class ConfirmarBoletosCommand : IRequest<Response<ConfirmarBoletosResultDto>>
    {
        public List<int> BoletoIds { get; set; } = new();
        public int IdCliente { get; set; }                  // cliente a facturar
        public decimal PrecioUnitario { get; set; }         // precio final por boleto
        public string MetodoPago { get; set; } = "EFECTIVO"; // EFECTIVO | QR | TRANSFERENCIA
        public string? ReferenciaPago { get; set; }         // obligatorio si QR/TRANSFERENCIA
        public int IdUsuario { get; set; } = 1;             // usuario operador
        public int ReservaTtlMinutos { get; set; } = 10;    // coherente con reserva
    }

    // ===== Resultado =====
    public class ConfirmarBoletosResultDto
    {
        public List<int> Ok { get; set; } = new();
        public List<ConfirmarBoletosFailItem> Fail { get; set; } = new();
        public decimal TotalCobrado { get; set; }
    }

    public class ConfirmarBoletosFailItem
    {
        public int Id { get; set; }
        public string Motivo { get; set; } = "";
    }

    // ===== Handler =====
    public class ConfirmarBoletosCommandHandler : IRequestHandler<ConfirmarBoletosCommand, Response<ConfirmarBoletosResultDto>>
    {
        private readonly IRepositoryAsync<Boleto> _boletoRepo;
        private readonly IRepositoryAsync<Pago> _pagoRepo;

        public ConfirmarBoletosCommandHandler(IRepositoryAsync<Boleto> boletoRepo, IRepositoryAsync<Pago> pagoRepo)
        {
            _boletoRepo = boletoRepo;
            _pagoRepo = pagoRepo;
        }

        public async Task<Response<ConfirmarBoletosResultDto>> Handle(ConfirmarBoletosCommand request, CancellationToken ct)
        {
            if (request.BoletoIds == null || request.BoletoIds.Count == 0)
                throw new InvalidOperationException("No hay boletos para confirmar.");
            if (request.PrecioUnitario <= 0m)
                throw new InvalidOperationException("Precio inválido.");
            if (string.Equals(request.MetodoPago, "QR", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(request.MetodoPago, "TRANSFERENCIA", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(request.ReferenciaPago))
                    throw new InvalidOperationException("La referencia es obligatoria para QR/TRANSFERENCIA.");
            }

            var ahora = DateTime.Now;
            var result = new ConfirmarBoletosResultDto();

            foreach (var id in request.BoletoIds.Distinct())
            {
                try
                {
                    var b = await _boletoRepo.GetByIdAsync(id);
                    if (b == null)
                    {
                        result.Fail.Add(new ConfirmarBoletosFailItem { Id = id, Motivo = "Boleto no encontrado." });
                        continue;
                    }

                    if (!string.Equals(b.Estado, "BLOQUEADO", StringComparison.OrdinalIgnoreCase))
                    {
                        result.Fail.Add(new ConfirmarBoletosFailItem { Id = id, Motivo = $"Estado inválido: {b.Estado}." });
                        continue;
                    }

                    // Reserva expirada no se confirma
                    var minutos = (ahora - b.FechaCompra).TotalMinutes;
                    if (minutos > request.ReservaTtlMinutos)
                    {
                        result.Fail.Add(new ConfirmarBoletosFailItem { Id = id, Motivo = "Reserva expirada." });
                        continue;
                    }

                    // Actualiza boleto a PAGADO (precio final)
                    b.Precio = request.PrecioUnitario;
                    b.IdCliente = request.IdCliente; // por si se actualiza el cliente facturado
                    b.Estado = "PAGADO";
                    b.FechaCompra = ahora;

                    await _boletoRepo.UpdateAsync(b);

                    // Crea pago
                    var pago = new Pago
                    {
                        TipoPago = "Boleto",
                        IdReferencia = b.IdBoleto,
                        Monto = request.PrecioUnitario,
                        Metodo = request.MetodoPago,
                        Referencia = request.ReferenciaPago, // si tu entidad lo tiene; si no, quita esta línea
                        FechaPago = ahora,
                        IdUsuario = request.IdUsuario,
                        IdCliente = request.IdCliente
                    };
                    await _pagoRepo.AddAsync(pago);

                    result.Ok.Add(id);
                    result.TotalCobrado += request.PrecioUnitario;
                }
                catch (Exception ex)
                {
                    result.Fail.Add(new ConfirmarBoletosFailItem { Id = id, Motivo = ex.Message });
                }
            }

            return new Response<ConfirmarBoletosResultDto>(result, "Confirmación procesada.");
        }
    }
}
