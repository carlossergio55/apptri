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

    public class GetAllEncomiendaQuery : IRequest<Response<List<EncomiendaDto>>>
    {
      

        public class GetAllEncomiendaQueryHandler : IRequestHandler<GetAllEncomiendaQuery, Response<List<EncomiendaDto>>>
        {
            private readonly IRepositoryAsync<Encomienda> _repositoryAsync;
            private readonly IMapper _mapper;
            public GetAllEncomiendaQueryHandler(IRepositoryAsync<Encomienda> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<Response<List<EncomiendaDto>>> Handle(GetAllEncomiendaQuery request, CancellationToken cancellationToken)
            {
                var _Encomienda = await _repositoryAsync.ListAsync();
                var _EncomiendaDto = _mapper.Map<List<EncomiendaDto>>(_Encomienda);
                return new Response<List<EncomiendaDto>>(_EncomiendaDto);
            }

           
        }

    }

  
}
