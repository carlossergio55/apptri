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
    public class GetAllParadaQuery : IRequest<Response<List<ParadaDto>>> { }

    public class GetAllParadaQueryHandler
        : IRequestHandler<GetAllParadaQuery, Response<List<ParadaDto>>>
    {
        private readonly IRepositoryAsync<Parada> _repo;
        private readonly IMapper _mapper;
        public GetAllParadaQueryHandler(IRepositoryAsync<Parada> repo, IMapper mapper)
            => (_repo, _mapper) = (repo, mapper);

        public async Task<Response<List<ParadaDto>>> Handle(GetAllParadaQuery request, CancellationToken ct)
        {
            var items = await _repo.ListAsync();
            var dtos = _mapper.Map<List<ParadaDto>>(items.OrderBy(x => x.Nombre));
            return new Response<List<ParadaDto>>(dtos);
        }
    }
}
