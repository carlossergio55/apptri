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
    public class ConfirmarBoletosCommand : IRequest<Response<ConfirmarBoletosResultDto>>
    {
        public List<int> BoletoIds { get; set; } = new();
        public int IdCliente { get; set; }
        public decimal PrecioUnitario { get; set; }
        public string MetodoPago { get; set; } = "EFECTIVO"; // EFECTIVO | QR | TRANSFERENCIA
        public string? ReferenciaPago { get; set; }
        public int IdUsuario { get; set; } = 1;
        public int ReservaTtlMinutos { get; set; } = 10;
    }

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

    public class ConfirmarBoletosCommandHandler : IRequestHandler<ConfirmarBoletosCommand, Response<ConfirmarBoletosResultDto>>
    {
        private readonly IRepositoryAsync<Boleto> _boletoRepo;
        private readonly IRepositoryAsync<Pago> _pagoRepo;
        private readonly IRepositoryAsync<Viaje> _viajeRepo;

        public ConfirmarBoletosCommandHandler(
            IRepositoryAsync<Boleto> boletoRepo,
            IRepositoryAsync<Pago> pagoRepo,
            IRepositoryAsync<Viaje> viajeRepo)
        {
            _boletoRepo = boletoRepo;
            _pagoRepo = pagoRepo;
            _viajeRepo = viajeRepo;
        }

        public async Task<Response<ConfirmarBoletosResultDto>> Handle(ConfirmarBoletosCommand request, CancellationToken ct)
        {
            if (request.BoletoIds == null || request.BoletoIds.Count == 0)
                throw new InvalidOperationException("No hay boletos para confirmar.");
            if (request.PrecioUnitario <= 0m)
                throw new InvalidOperationException("Precio inválido.");
            if ((string.Equals(request.MetodoPago, "QR", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(request.MetodoPago, "TRANSFERENCIA", StringComparison.OrdinalIgnoreCase)) &&
                string.IsNullOrWhiteSpace(request.ReferenciaPago))
                throw new InvalidOperationException("La referencia es obligatoria para QR/TRANSFERENCIA.");

            var ahora = DateTime.Now; // **hora local** para ser consistente con Reservar/Expirar
            var result = new ConfirmarBoletosResultDto();

            foreach (var id in request.BoletoIds.Distinct())
            {
                try
                {
                    var b = await _boletoRepo.GetByIdAsync(id);
                    if (b is null)
                    {
                        result.Fail.Add(new ConfirmarBoletosFailItem { Id = id, Motivo = "Boleto no encontrado." });
                        continue;
                    }

                    // Estados permitidos
                    if (string.Equals(b.Estado, "ANULADO", StringComparison.OrdinalIgnoreCase))
                    {
                        result.Fail.Add(new ConfirmarBoletosFailItem { Id = id, Motivo = "La reserva fue anulada." });
                        continue;
                    }

                    // Idempotencia: si ya está pagado, lo consideramos OK (no volver a cobrar)
                    if (string.Equals(b.Estado, "PAGADO", StringComparison.OrdinalIgnoreCase))
                    {
                        result.Ok.Add(id);
                        continue;
                    }

                    if (!string.Equals(b.Estado, "BLOQUEADO", StringComparison.OrdinalIgnoreCase))
                    {
                        result.Fail.Add(new ConfirmarBoletosFailItem { Id = id, Motivo = $"Estado inválido: {b.Estado}." });
                        continue;
                    }

                    // TTL: preferir FechaReservaUtc; fallback a FechaCompra; si tampoco hay, ahora (=> 0 min)
                    var fechaBase = b.FechaReservaUtc ?? b.FechaCompra ?? ahora;
                    var minutos = (ahora - fechaBase).TotalMinutes;
                    if (minutos > request.ReservaTtlMinutos)
                    {
                        result.Fail.Add(new ConfirmarBoletosFailItem { Id = id, Motivo = "Reserva expirada por tiempo de espera." });
                        continue;
                    }

                    // Ventana T–2h: no confirmar si faltan ≤ 2 horas para la salida
                    var viaje = await _viajeRepo.GetByIdAsync(b.IdViaje);
                    if (viaje is null)
                    {
                        result.Fail.Add(new ConfirmarBoletosFailItem { Id = id, Motivo = "Viaje no encontrado." });
                        continue;
                    }
                    var salidaLocal = viaje.Fecha.Date + viaje.HoraSalida; // DateOnly + TimeSpan => DateTime local
                    var limite = salidaLocal.AddHours(-2);
                    if (ahora >= limite)
                    {
                        result.Fail.Add(new ConfirmarBoletosFailItem { Id = id, Motivo = "La reserva se liberó por proximidad a la salida (≤ 2 horas)." });
                        continue;
                    }

                    // Confirmar pago
                    b.Precio = request.PrecioUnitario;
                    b.IdCliente = request.IdCliente;
                    b.Estado = "PAGADO";
                    b.FechaConfirmacionUtc = ahora; // sellado en hora local, coherente con el resto

                    await _boletoRepo.UpdateAsync(b);

                    var pago = new Pago
                    {
                        TipoPago = "Boleto",
                        IdReferencia = b.IdBoleto,
                        Monto = request.PrecioUnitario,
                        Metodo = request.MetodoPago,
                        Referencia = request.ReferenciaPago,
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
