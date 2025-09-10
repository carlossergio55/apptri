using Aplicacion.DTOs.Integracion;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Ardalis.Specification;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System.Linq;                 // <-- IMPORTANTE para FirstOrDefault()
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Queries
{
    // Request público para tracking por código de guía
    public class EncomiendaGuiaQuery : IRequest<Response<EncomiendaPublicDto>>
    {
        public string Guiacarga { get; set; } = string.Empty;
    }

    public class EncomiendaGuiaQueryHandler
        : IRequestHandler<EncomiendaGuiaQuery, Response<EncomiendaPublicDto>>
    {
        private readonly IRepositoryAsync<Encomienda> _repo;
        private readonly IMapper _mapper;

        public EncomiendaGuiaQueryHandler(IRepositoryAsync<Encomienda> repo, IMapper mapper)
            => (_repo, _mapper) = (repo, mapper);

        public async Task<Response<EncomiendaPublicDto>> Handle(EncomiendaGuiaQuery request, CancellationToken ct)
        {
            var list = await _repo.ListAsync(new EncomiendaPorGuiaSpec(request.Guiacarga), ct);
            var item = list.FirstOrDefault(); // <- en memoria

            if (item == null)
                return new Response<EncomiendaPublicDto>(null) { Message = "No encontrada" };

            var dto = _mapper.Map<EncomiendaPublicDto>(item);
            return new Response<EncomiendaPublicDto>(dto);
        }
    }

    // Spec para filtrar por guía + includes que necesita el DTO público
    public class EncomiendaPorGuiaSpec : Specification<Encomienda>
    {
        public EncomiendaPorGuiaSpec(string guiacarga)
        {
            var code = (guiacarga ?? string.Empty).Trim();
            Query.Where(x => x.Guiacarga == code);

            Query.Include(x => x.Viaje);
            Query.Include(x => x.OrigenParada);
            Query.Include(x => x.DestinoParada);

            Query.OrderByDescending(x => x.IdEncomienda);
        }
    }
}
