using Aplicacion.DTOs.Integracion;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Ardalis.Specification;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Aplicacion.Features.Integracion.Queries
{
    public record GetAllEncomiendaGuiaQuery(string? Guiacarga = null) : IRequest<Response<List<EncomiendaDto>>>;

    public class GetAllEncomiendaQueryHandler
    : IRequestHandler<GetAllEncomiendaGuiaQuery, Response<List<EncomiendaDto>>>
    {
        private readonly IRepositoryAsync<Encomienda> _repo;
        private readonly IMapper _mapper;

        public GetAllEncomiendaQueryHandler(IRepositoryAsync<Encomienda> repo, IMapper mapper)
            => (_repo, _mapper) = (repo, mapper);

        public async Task<Response<List<EncomiendaDto>>> Handle(GetAllEncomiendaGuiaQuery request, CancellationToken cancellationToken)
        {
            var list = await _repo.ListAsync(
                new EncomiendaConFiltroSpec(request.Guiacarga),
                cancellationToken
            );

            var dtoList = _mapper.Map<List<EncomiendaDto>>(list);
            return new Response<List<EncomiendaDto>>(dtoList);
        }
    }
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
