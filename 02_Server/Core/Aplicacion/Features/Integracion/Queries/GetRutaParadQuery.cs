using Aplicacion.DTOs.Integracion;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Queries
{
    // GET: todas las asociaciones Ruta–Parada (ordenadas por Ruta y Orden)
    public class GetAllRutaParadaQuery : IRequest<Response<List<RutaParadaDto>>> { }

    public class GetAllRutaParadaQueryHandler
        : IRequestHandler<GetAllRutaParadaQuery, Response<List<RutaParadaDto>>>
    {
        private readonly IRepositoryAsync<RutaParada> _repo;
        private readonly IMapper _mapper;

        public GetAllRutaParadaQueryHandler(IRepositoryAsync<RutaParada> repo, IMapper mapper)
            => (_repo, _mapper) = (repo, mapper);

        public async Task<Response<List<RutaParadaDto>>> Handle(GetAllRutaParadaQuery request, CancellationToken ct)
        {
            var items = (await _repo.ListAsync())
                        .OrderBy(x => x.IdRuta)
                        .ThenBy(x => x.Orden)
                        .ToList();

            var dtos = _mapper.Map<List<RutaParadaDto>>(items);
            return new Response<List<RutaParadaDto>>(dtos);
        }
    }
}
