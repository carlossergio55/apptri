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

            var estado = (b.Estado ?? "").ToUpperInvariant();

            // No se cancelan boletos pagados
            if (estado == "PAGADO")
                throw new InvalidOperationException("No se puede cancelar un boleto pagado. Use reprogramar.");

            // Solo se pueden cancelar RESERVADO o BLOQUEADO
            if (estado is not ("RESERVADO" or "BLOQUEADO"))
                throw new InvalidOperationException($"Estado inválido para cancelar: {b.Estado ?? "-"}.");

            // HARD DELETE (borra totalmente el registro)
            await _boletoRepo.DeleteAsync(b);

            return new Response<int>(request.IdBoleto, "Reserva/bloqueo eliminado.");
        }
    }
}
