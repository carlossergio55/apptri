using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Aplicacion.Wrappers;
using Aplicacion.Interfaces;
using Dominio.Entities.Integracion;

namespace Aplicacion.Features.Integracion.Commands.BoletoC
{
    public class CancelarReservaCommand : IRequest<Response<int>>
    {
        public int IdBoleto { get; set; }
        public int? IdUsuario { get; set; }   // opcional para auditoría
        public string? Motivo { get; set; }   // opcional para auditoría
    }

    public class CancelarReservaCommandHandler : IRequestHandler<CancelarReservaCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Boleto> _boletoRepo;

        public CancelarReservaCommandHandler(IRepositoryAsync<Boleto> boletoRepo)
        {
            _boletoRepo = boletoRepo;
        }

        public async Task<Response<int>> Handle(CancelarReservaCommand request, CancellationToken ct)
        {
            var b = await _boletoRepo.GetByIdAsync(request.IdBoleto);
            if (b is null)
                throw new InvalidOperationException("Boleto no encontrado.");

            if (string.Equals(b.Estado, "ANULADO", StringComparison.OrdinalIgnoreCase))
                return new Response<int>(b.IdBoleto, "La reserva ya estaba anulada.");

            // No se cancelan boletos pagados
            if (string.Equals(b.Estado, "PAGADO", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("No se puede cancelar un boleto pagado. Use reprogramar.");

            // Solo reservas (bloqueos) pueden cancelarse
            if (!string.Equals(b.Estado, "BLOQUEADO", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Estado inválido para cancelar: {b.Estado}.");

            // Anular reserva
            b.Estado = "ANULADO";

            await _boletoRepo.UpdateAsync(b);

            // TODO (opcional): guardar historial de auditoría con request.IdUsuario y request.Motivo

            return new Response<int>(b.IdBoleto, "Reserva anulada correctamente.");
        }
    }
}
