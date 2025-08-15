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
        public int IdRuta { get; set; }
        public int OrigenParadaId { get; set; }
        public int DestinoParadaId { get; set; }
        public decimal Precio { get; set; }
    }

    public class UpdateTarifaTramoCommandHandler : IRequestHandler<UpdateTarifaTramoCommand, Response<int>>
    {
        private readonly IRepositoryAsync<TarifaTramo> _repo;
        public UpdateTarifaTramoCommandHandler(IRepositoryAsync<TarifaTramo> repo) => _repo = repo;

        public async Task<Response<int>> Handle(UpdateTarifaTramoCommand request, CancellationToken ct)
        {
            var all = await _repo.ListAsync();
            var entity = all.FirstOrDefault(x =>
                x.IdRuta == request.IdRuta &&
                x.OrigenParadaId == request.OrigenParadaId &&
                x.DestinoParadaId == request.DestinoParadaId);

            if (entity == null) throw new KeyNotFoundException("Registro no encontrado.");

            entity.Precio = request.Precio;

            await _repo.UpdateAsync(entity);
            return new Response<int>(1); // PK compuesta
        }
    }
}
