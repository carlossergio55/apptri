using Aplicacion.DTOs.Integracion;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.ParadaC
{
    public class CreateParadaCommand : IRequest<Response<int>>
    {
        public ParadaDto Parada { get; set; } = null!;
    }

    public class CreateParadaCommandHandler : IRequestHandler<CreateParadaCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Parada> _repo;
        private readonly IMapper _mapper;
        public CreateParadaCommandHandler(IRepositoryAsync<Parada> repo, IMapper mapper)
            => (_repo, _mapper) = (repo, mapper);

        public async Task<Response<int>> Handle(CreateParadaCommand request, CancellationToken ct)
        {
            var entity = _mapper.Map<Parada>(request.Parada);
            var saved = await _repo.AddAsync(entity);
            return new Response<int>(saved.IdParada);
        }
    }
}
