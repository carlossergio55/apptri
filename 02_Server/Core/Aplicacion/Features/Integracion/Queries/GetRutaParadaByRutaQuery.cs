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
    public class GetRutaParadaByRutaQuery : IRequest<Response<List<RutaParadaDto>>>
    {
        public int IdRuta { get; set; }
    }

    public class GetRutaParadaByRutaQueryHandler
        : IRequestHandler<GetRutaParadaByRutaQuery, Response<List<RutaParadaDto>>>
    {
        private readonly IRepositoryAsync<RutaParada> _repo;
        private readonly IMapper _mapper;
        public GetRutaParadaByRutaQueryHandler(IRepositoryAsync<RutaParada> repo, IMapper mapper)
            => (_repo, _mapper) = (repo, mapper);

        public async Task<Response<List<RutaParadaDto>>> Handle(GetRutaParadaByRutaQuery request, CancellationToken ct)
        {
            var items = (await _repo.ListAsync())
                        .Where(x => x.IdRuta == request.IdRuta)
                        .OrderBy(x => x.Orden)
                        .ToList();
            var dtos = _mapper.Map<List<RutaParadaDto>>(items);
            return new Response<List<RutaParadaDto>>(dtos);
        }
    }
}
