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

    public class GetAllRutaQuery : IRequest<Response<List<RutaDto>>>
    {
       

        public class GetAllRutaQueryHandler : IRequestHandler<GetAllRutaQuery, Response<List<RutaDto>>>
        {
            private readonly IRepositoryAsync<Ruta> _repositoryAsync;
            private readonly IMapper _mapper;
            public GetAllRutaQueryHandler(IRepositoryAsync<Ruta> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<Response<List<RutaDto>>> Handle(GetAllRutaQuery request, CancellationToken cancellationToken)
            {
                var _Ruta = await _repositoryAsync.ListAsync();
                var _RutaDto = _mapper.Map<List<RutaDto>>(_Ruta);
                return new Response<List<RutaDto>>(_RutaDto);
            }

        
        }

    }

  
}
