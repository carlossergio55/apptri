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

    public class GetAllHorarioQuery : IRequest<Response<List<HorarioDto>>>
    {
 

        public class GetAllHorarioQueryHandler : IRequestHandler<GetAllHorarioQuery, Response<List<HorarioDto>>>
        {
            private readonly IRepositoryAsync<Horario> _repositoryAsync;
            private readonly IMapper _mapper;
            public GetAllHorarioQueryHandler(IRepositoryAsync<Horario> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<Response<List<HorarioDto>>> Handle(GetAllHorarioQuery request, CancellationToken cancellationToken)
            {
                var _Horario = await _repositoryAsync.ListAsync();
                var _HorarioDto = _mapper.Map<List<HorarioDto>>(_Horario);
                return new Response<List<HorarioDto>>(_HorarioDto);
            }

          
        }

    }

    
}
