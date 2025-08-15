using Aplicacion.DTOs.Integracion;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Queries
{
    public class GetTarifaDeTramoActualQuery : IRequest<Response<TarifaTramoDto?>>
    {
        public int IdRuta { get; set; }
        public int OrigenParadaId { get; set; }
        public int DestinoParadaId { get; set; }
    }

    public class GetTarifaDeTramoActualQueryHandler
        : IRequestHandler<GetTarifaDeTramoActualQuery, Response<TarifaTramoDto?>>
    {
        private readonly IRepositoryAsync<TarifaTramo> _repo;
        private readonly IMapper _mapper;
        public GetTarifaDeTramoActualQueryHandler(IRepositoryAsync<TarifaTramo> repo, IMapper mapper)
            => (_repo, _mapper) = (repo, mapper);

        public async Task<Response<TarifaTramoDto?>> Handle(GetTarifaDeTramoActualQuery request, CancellationToken ct)
        {
            var item = (await _repo.ListAsync())
                       .FirstOrDefault(x => x.IdRuta == request.IdRuta
                                         && x.OrigenParadaId == request.OrigenParadaId
                                         && x.DestinoParadaId == request.DestinoParadaId);

            var dto = item is null ? null : _mapper.Map<TarifaTramoDto>(item);
            return new Response<TarifaTramoDto?>(dto);
        }
    }
}
