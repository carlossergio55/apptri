using Aplicacion.DTOs.Integracion;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Ardalis.Specification;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Queries
{
    // 1️⃣ Record de la Query con parámetro opcional
    public record GetAllRutaParadaQuery(int? IdRuta = null) : IRequest<Response<List<RutaParadaDto>>>;

    // 2️⃣ Handler
    public class GetAllRutaParadaQueryHandler
        : IRequestHandler<GetAllRutaParadaQuery, Response<List<RutaParadaDto>>>
    {
        private readonly IRepositoryAsync<RutaParada> _repo;
        private readonly IMapper _mapper;

        public GetAllRutaParadaQueryHandler(IRepositoryAsync<RutaParada> repo, IMapper mapper)
            => (_repo, _mapper) = (repo, mapper);

        public async Task<Response<List<RutaParadaDto>>> Handle(GetAllRutaParadaQuery request, CancellationToken cancellationToken)
        {
            var list = await _repo.ListAsync(
                new RutaParadaConNavegacionSpec(request.IdRuta),
                cancellationToken
            );

            var dtoList = _mapper.Map<List<RutaParadaDto>>(list);
            return new Response<List<RutaParadaDto>>(dtoList);
        }
    }

    // 3️⃣ Especificación
    public class RutaParadaConNavegacionSpec : Specification<RutaParada>
    {
        public RutaParadaConNavegacionSpec(int? idRuta)
        {
            Query.Include(x => x.Ruta)
                 .Include(x => x.Parada);

            if (idRuta.HasValue)
                Query.Where(x => x.IdRuta == idRuta.Value);

            Query.OrderBy(x => x.IdRuta)
                 .ThenBy(x => x.Orden);
        }
    }
}
