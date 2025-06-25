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

    public class GetAllChoferQuery : IRequest<Response<List<ChoferDto>>>
    {
       


        public class GetAllChoferQueryHandler : IRequestHandler<GetAllChoferQuery, Response<List<ChoferDto>>>
        {
            private readonly IRepositoryAsync<Chofer> _repositoryAsync;
            private readonly IMapper _mapper;
            public GetAllChoferQueryHandler(IRepositoryAsync<Chofer> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<Response<List<ChoferDto>>> Handle(GetAllChoferQuery request, CancellationToken cancellationToken)
            {
                var _Chofer = await _repositoryAsync.ListAsync();
                var _ChoferDto = _mapper.Map<List<ChoferDto>>(_Chofer);
                return new Response<List<ChoferDto>>(_ChoferDto);
            }

          
        }

    }


}
