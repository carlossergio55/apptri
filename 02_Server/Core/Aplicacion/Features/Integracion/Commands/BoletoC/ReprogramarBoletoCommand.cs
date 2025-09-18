using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.BoletoC
{
    // El Command hace de contrato del endpoint
    public class ReprogramarBoletoCommand : IRequest<Response<int>>
    {
        public int IdBoleto { get; set; }
        public int IdViajeDestino { get; set; }
        public int IdAsientoDestino { get; set; }
        public int? IdUsuario { get; set; }
        public string? Motivo { get; set; }
    }

    public class ReprogramarBoletoCommandHandler : IRequestHandler<ReprogramarBoletoCommand, Response<int>>
    {
        private readonly IApplicationDbContext _ctx;
        public ReprogramarBoletoCommandHandler(IApplicationDbContext ctx) => _ctx = ctx;

        public async Task<Response<int>> Handle(ReprogramarBoletoCommand request, CancellationToken ct)
        {
            // 1) Boleto origen
            var boleto = await _ctx.Boleto.FirstOrDefaultAsync(b => b.IdBoleto == request.IdBoleto, ct);
            if (boleto is null) throw new KeyNotFoundException("Boleto no encontrado.");

            if (string.Equals(boleto.Estado, "ANULADO", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("No se puede reprogramar un boleto ANULADO.");

            // 2) Validar disponibilidad en destino: ocupado si hay PAGADO o BLOQUEADO
            var ocupado = await _ctx.Boleto.AnyAsync(b =>
                b.IdViaje == request.IdViajeDestino &&
                b.IdAsiento == request.IdAsientoDestino &&
                (b.Estado.ToUpper() == "PAGADO" || b.Estado.ToUpper() == "BLOQUEADO"), ct);

            if (ocupado) throw new InvalidOperationException("El asiento destino no está disponible.");

            // 3) Mover el boleto (conserva estado y pagos)
            boleto.IdViaje = request.IdViajeDestino;
            boleto.IdAsiento = request.IdAsientoDestino;
            boleto.FechaModificacion = DateTime.Now;
            boleto.UsuarioModificacion = request.IdUsuario?.ToString();

            await _ctx.SaveChangesAsync(ct);
            return new Response<int>(boleto.IdBoleto, "Boleto reprogramado.");
        }
    }
}
