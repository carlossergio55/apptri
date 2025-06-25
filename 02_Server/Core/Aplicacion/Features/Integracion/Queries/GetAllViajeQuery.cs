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

    public class GetAllViajeQuery : IRequest<Response<List<ViajeDto>>>
    {
     


        public class GetAllViajeQueryHandler : IRequestHandler<GetAllViajeQuery, Response<List<ViajeDto>>>
        {
            private readonly IRepositoryAsync<Viaje> _repositoryAsync;
            private readonly IMapper _mapper;
            public GetAllViajeQueryHandler(IRepositoryAsync<Viaje> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<Response<List<ViajeDto>>> Handle(GetAllViajeQuery request, CancellationToken cancellationToken)
            {
                var _Viaje = await _repositoryAsync.ListAsync();
                var _ViajeDto = _mapper.Map<List<ViajeDto>>(_Viaje);
                return new Response<List<ViajeDto>>(_ViajeDto);
            }

         
        }

    }

  
}
