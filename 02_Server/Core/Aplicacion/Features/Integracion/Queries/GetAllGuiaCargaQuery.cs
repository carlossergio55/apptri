
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
    public class GetAllGuiaCargaQuery : IRequest<Response<List<GuiaCargaDto>>> { }

    public class GetAllGuiaCargaQueryHandler
        : IRequestHandler<GetAllGuiaCargaQuery, Response<List<GuiaCargaDto>>>
    {
        private readonly IRepositoryAsync<GuiaCarga> _repo;
        private readonly IMapper _mapper;
        public GetAllGuiaCargaQueryHandler(IRepositoryAsync<GuiaCarga> repo, IMapper mapper)
            => (_repo, _mapper) = (repo, mapper);

        public async Task<Response<List<GuiaCargaDto>>> Handle(GetAllGuiaCargaQuery request, CancellationToken ct)
        {
            var items = await _repo.ListAsync();
            var dtos = _mapper.Map<List<GuiaCargaDto>>(items.OrderBy(x => x.IdGuiaCarga));
            return new Response<List<GuiaCargaDto>>(dtos);
        }
    }
}
