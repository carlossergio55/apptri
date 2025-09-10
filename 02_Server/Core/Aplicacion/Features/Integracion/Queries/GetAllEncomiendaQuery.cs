// GetAllEncomiendaQuery.cs
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
    // guiacarga opcional
    public class GetAllEncomiendaQuery : IRequest<Response<List<EncomiendaDto>>>
    {
        public string? Guiacarga { get; init; }
    }

    public class GetAllEncomiendaQueryHandler
        : IRequestHandler<GetAllEncomiendaQuery, Response<List<EncomiendaDto>>>
    {
        private readonly IRepositoryAsync<Encomienda> _repo;
        private readonly IMapper _mapper;

        public GetAllEncomiendaQueryHandler(IRepositoryAsync<Encomienda> repo, IMapper mapper)
            => (_repo, _mapper) = (repo, mapper);

        public async Task<Response<List<EncomiendaDto>>> Handle(GetAllEncomiendaQuery request, CancellationToken ct)
        {
            var items = await _repo.ListAsync(new EncomiendaConFiltroSpec(request.Guiacarga), ct);
            var dtos = _mapper.Map<List<EncomiendaDto>>(items);
            return new Response<List<EncomiendaDto>>(dtos);
        }
    }

    // Solo filtra cuando guiacarga tiene valor
    public class EncomiendaConFiltroSpec : Specification<Encomienda>
    {
        public EncomiendaConFiltroSpec(string? guiacarga)
        {
            if (!string.IsNullOrWhiteSpace(guiacarga))
                Query.Where(x => x.Guiacarga == guiacarga);

            Query.OrderByDescending(x => x.IdEncomienda);
        }
    }
}
