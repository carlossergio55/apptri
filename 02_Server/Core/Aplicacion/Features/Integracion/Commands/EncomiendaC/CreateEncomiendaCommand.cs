using Aplicacion.DTOs.Integracion;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.EncomiendaC
{
    public class CreateEncomiendaCommand : IRequest<Response<int>>
    {
        public EncomiendaDto Encomienda { get; set; } = null!;
    }

    public class CreateEncomiendaCommandHandler : IRequestHandler<CreateEncomiendaCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Encomienda> _repositoryAsync;
        private readonly IMapper _mapper;

        public CreateEncomiendaCommandHandler(IRepositoryAsync<Encomienda> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateEncomiendaCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Encomienda>(request.Encomienda);

            

            var saved = await _repositoryAsync.AddAsync(entity);
            return new Response<int>(saved.IdEncomienda);
        }
    }
}
