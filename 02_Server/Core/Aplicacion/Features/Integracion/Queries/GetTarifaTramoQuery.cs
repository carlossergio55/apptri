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
    public class GetTarifaTramoQuery : IRequest<Response<List<TarifaTramoDto>>>
    {
        public int IdRuta { get; set; }
        public int? OrigenParadaId { get; set; }
        public int? DestinoParadaId { get; set; }
    }

    public class GetTarifaTramoQueryHandler
        : IRequestHandler<GetTarifaTramoQuery, Response<List<TarifaTramoDto>>>
    {
        private readonly IRepositoryAsync<TarifaTramo> _repo;
        private readonly IMapper _mapper;

        public GetTarifaTramoQueryHandler(IRepositoryAsync<TarifaTramo> repo, IMapper mapper)
            => (_repo, _mapper) = (repo, mapper);

        public async Task<Response<List<TarifaTramoDto>>> Handle(GetTarifaTramoQuery request, CancellationToken ct)
        {
            var all = await _repo.ListAsync(ct);             // igual que tu GetAllBusQuery
            var q = all.Where(x => x.IdRuta == request.IdRuta);

            if (request.OrigenParadaId.HasValue)
                q = q.Where(x => x.OrigenParadaId == request.OrigenParadaId.Value);

            if (request.DestinoParadaId.HasValue)
                q = q.Where(x => x.DestinoParadaId == request.DestinoParadaId.Value);

            var dtos = _mapper.Map<List<TarifaTramoDto>>(q.ToList());
            return new Response<List<TarifaTramoDto>>(dtos);
        }
    }
}
