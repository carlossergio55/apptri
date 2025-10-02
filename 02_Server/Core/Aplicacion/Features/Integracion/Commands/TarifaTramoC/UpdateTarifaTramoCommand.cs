using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Integracion;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.TarifaTramoC
{
    public class UpdateTarifaTramoCommand : IRequest<Response<int>>
    {
        public int IdTarifaTramo { get; set; }     // ✅ PK única
        public int IdRuta { get; set; }
        public int OrigenParadaId { get; set; }
        public int DestinoParadaId { get; set; }
        public decimal Precio { get; set; }
    }

    public class UpdateTarifaTramoCommandHandler
        : IRequestHandler<UpdateTarifaTramoCommand, Response<int>>
    {
        private readonly IRepositoryAsync<TarifaTramo> _repo;

        public UpdateTarifaTramoCommandHandler(IRepositoryAsync<TarifaTramo> repo) => _repo = repo;

        public async Task<Response<int>> Handle(UpdateTarifaTramoCommand request, CancellationToken ct)
        {
            // 1) Buscar por PK
            var entity = await _repo.GetByIdAsync(request.IdTarifaTramo);
            if (entity is null)
                throw new KeyNotFoundException($"TarifaTramo {request.IdTarifaTramo} no encontrada.");

            // 2) (Opcional pero recomendado) Evitar duplicados si cambian los extremos/ruta
            if (entity.IdRuta != request.IdRuta ||
                entity.OrigenParadaId != request.OrigenParadaId ||
                entity.DestinoParadaId != request.DestinoParadaId)
            {
                var all = await _repo.ListAsync(ct);
                var existeDuplicado = all.Any(x =>
                    x.IdTarifaTramo != request.IdTarifaTramo &&
                    x.IdRuta == request.IdRuta &&
                    x.OrigenParadaId == request.OrigenParadaId &&
                    x.DestinoParadaId == request.DestinoParadaId);

                if (existeDuplicado)
                    throw new System.InvalidOperationException("Ya existe una tarifa para ese tramo.");
            }

            // 3) Actualizar campos
            entity.IdRuta = request.IdRuta;
            entity.OrigenParadaId = request.OrigenParadaId;
            entity.DestinoParadaId = request.DestinoParadaId;
            entity.Precio = request.Precio;

            await _repo.UpdateAsync(entity);

            // 4) Devolver el Id actualizado
            return new Response<int>(entity.IdTarifaTramo);
        }
    }
}
