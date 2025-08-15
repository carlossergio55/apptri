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

    public class CreateTarifaTramoCommandHandler : IRequestHandler<CreateTarifaTramoCommand, Response<int>>
    {
        private readonly IRepositoryAsync<TarifaTramo> _repo;
        private readonly IRepositoryAsync<RutaParada> _rp;
        private readonly IMapper _mapper;

        public CreateTarifaTramoCommandHandler(IRepositoryAsync<TarifaTramo> repo,
                                               IRepositoryAsync<RutaParada> rp,
                                               IMapper mapper)
        { _repo = repo; _rp = rp; _mapper = mapper; }

        public async Task<Response<int>> Handle(CreateTarifaTramoCommand request, CancellationToken ct)
        {
            // validar que ambos paradas pertenecen a la ruta
            var paradasRuta = (await _rp.ListAsync()).Where(x => x.IdRuta == request.Tarifa.IdRuta).ToList();
            if (!paradasRuta.Any(p => p.IdParada == request.Tarifa.OrigenParadaId) ||
                !paradasRuta.Any(p => p.IdParada == request.Tarifa.DestinoParadaId))
                throw new Exception("Origen/Destino no pertenecen a la ruta.");

            // evitar duplicado
            var existentes = await _repo.ListAsync();
            if (existentes.Any(x => x.IdRuta == request.Tarifa.IdRuta &&
                                    x.OrigenParadaId == request.Tarifa.OrigenParadaId &&
                                    x.DestinoParadaId == request.Tarifa.DestinoParadaId))
                throw new Exception("Ya existe una tarifa para ese tramo.");

            var entity = _mapper.Map<TarifaTramo>(request.Tarifa);
            await _repo.AddAsync(entity);
            return new Response<int>(1);
        }
    }
}
