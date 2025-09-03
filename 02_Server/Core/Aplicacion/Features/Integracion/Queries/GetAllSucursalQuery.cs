using Aplicacion.DTOs.Integracion;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Queries
{
    public class GetAllSucursalQuery : IRequest<Response<List<SucursalDto>>>
    {
        public class GetAllSucursalQueryHandler : IRequestHandler<GetAllSucursalQuery, Response<List<SucursalDto>>>
        {
            private readonly IRepositoryAsync<Sucursal> _repositoryAsync;
            private readonly IMapper _mapper;

            public GetAllSucursalQueryHandler(IRepositoryAsync<Sucursal> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<Response<List<SucursalDto>>> Handle(GetAllSucursalQuery request, CancellationToken cancellationToken)
            {
                var sucursales = await _repositoryAsync.ListAsync();
                var sucursalDtos = _mapper.Map<List<SucursalDto>>(sucursales);
                return new Response<List<SucursalDto>>(sucursalDtos);
            }
        }
    }
}
