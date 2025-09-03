using Aplicacion.DTOs.Integracion;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.RutaParadaC
{
    public class CreateRutaParadaCommand : IRequest<Response<int>>
    {
        public RutaParadaDto RutaParada { get; set; }
    }

    public class CreateRutaParadaCommandHandler : IRequestHandler<CreateRutaParadaCommand, Response<int>>
    {
        private readonly IRepositoryAsync<RutaParada> _repositoryAsync;
        private readonly IMapper _mapper;

        public CreateRutaParadaCommandHandler(IRepositoryAsync<RutaParada> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateRutaParadaCommand request, CancellationToken cancellationToken)
        {
            var nuevoRegistro = _mapper.Map<RutaParada>(request.RutaParada);
            var data = await _repositoryAsync.AddAsync(nuevoRegistro);
            return new Response<int>(data.IdRutaParada);
        }
    }
}
