using Aplicacion.DTOs.Integracion;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Queries
{

    public class GetAllBusQuery : IRequest<Response<List<BusDto>>>
    {
        


        public class GetAllBusQueryHandler : IRequestHandler<GetAllBusQuery, Response<List<BusDto>>>
        {
            private readonly IRepositoryAsync<Bus> _repositoryAsync;
            private readonly IMapper _mapper;
            public GetAllBusQueryHandler(IRepositoryAsync<Bus> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<Response<List<BusDto>>> Handle(GetAllBusQuery request, CancellationToken cancellationToken)
            {
                var _Bus = await _repositoryAsync.ListAsync();
                var _BusDto = _mapper.Map<List<BusDto>>(_Bus);
                return new Response<List<BusDto>>(_BusDto);
            }

        }

    }

 
}
