using Aplicacion.DTOs.Integracion;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.TarifaTramoC
{
    public class CreateTarifaTramoCommand : IRequest<Response<int>>
    {
        public TarifaTramoDto Tarifa { get; set; } = null!;
    }

    public class CreateTarifaTramoCommandHandler
        : IRequestHandler<CreateTarifaTramoCommand, Response<int>>
    {
        private readonly IRepositoryAsync<TarifaTramo> _repo;
        private readonly IRepositoryAsync<RutaParada> _rp;
        private readonly IMapper _mapper;

        public CreateTarifaTramoCommandHandler(
            IRepositoryAsync<TarifaTramo> repo,
            IRepositoryAsync<RutaParada> rp,
            IMapper mapper)
        { _repo = repo; _rp = rp; _mapper = mapper; }

        public async Task<Response<int>> Handle(CreateTarifaTramoCommand request, CancellationToken ct)
        {
            if (request is null || request.Tarifa is null)
                throw new ArgumentNullException(nameof(request.Tarifa));

            var t = request.Tarifa;

            // Validaciones básicas
            if (t.IdRuta <= 0) throw new ArgumentException("IdRuta inválido.");
            if (t.OrigenParadaId <= 0 || t.DestinoParadaId <= 0)
                throw new ArgumentException("Paradas inválidas.");
            if (t.OrigenParadaId == t.DestinoParadaId)
                throw new ArgumentException("El origen y destino no pueden ser iguales.");
            if (t.Precio <= 0)
                throw new ArgumentException("El precio debe ser mayor a 0.");

            // Paradas pertenecen a la ruta
            var paradasRuta = (await _rp.ListAsync(ct)).Where(x => x.IdRuta == t.IdRuta).ToList();
            var ori = paradasRuta.FirstOrDefault(p => p.IdParada == t.OrigenParadaId);
            var des = paradasRuta.FirstOrDefault(p => p.IdParada == t.DestinoParadaId);

            if (ori is null || des is null)
                throw new InvalidOperationException("Origen/Destino no pertenecen a la ruta.");

            // Orden dentro de la ruta (origen antes que destino)
            if (ori.Orden >= des.Orden)
                throw new InvalidOperationException("Orden inválido: el origen debe ir antes que el destino en la ruta.");

            // Evitar duplicado del tramo (ruta + origen + destino)
            var existentes = await _repo.ListAsync(ct);
            if (existentes.Any(x => x.IdRuta == t.IdRuta
                                 && x.OrigenParadaId == t.OrigenParadaId
                                 && x.DestinoParadaId == t.DestinoParadaId))
                throw new InvalidOperationException("Ya existe una tarifa para ese tramo.");

            // Crear
            var entity = _mapper.Map<TarifaTramo>(t);
            await _repo.AddAsync(entity, ct); // si tu repo no acepta CT, usa AddAsync(entity)

            // EF debería hidratar el Id generado
            return new Response<int>(entity.IdTarifaTramo);
        }
    }
}
