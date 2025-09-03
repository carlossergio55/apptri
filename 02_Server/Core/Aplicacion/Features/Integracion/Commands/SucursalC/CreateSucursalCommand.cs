using Aplicacion.DTOs.Integracion;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.SucursalC
{
    public class CreateSucursalCommand : IRequest<Response<int>>
    {
        public SucursalDto Sucursal { get; set; }
    }

    public class CreateSucursalCommandHandler : IRequestHandler<CreateSucursalCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Sucursal> _repositoryAsync;
        private readonly IMapper _mapper;

        public CreateSucursalCommandHandler(IRepositoryAsync<Sucursal> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateSucursalCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Sucursal>(request.Sucursal);

          

            var created = await _repositoryAsync.AddAsync(entity);
            return new Response<int>(created.IdSucursal);
        }
    }
}
