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

    public class GetAllClienteQuery : IRequest<Response<List<ClienteDto>>>
    {
    


        public class GetAllClienteQueryHandler : IRequestHandler<GetAllClienteQuery, Response<List<ClienteDto>>>
        {
            private readonly IRepositoryAsync<Cliente> _repositoryAsync;
            private readonly IMapper _mapper;
            public GetAllClienteQueryHandler(IRepositoryAsync<Cliente> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<Response<List<ClienteDto>>> Handle(GetAllClienteQuery request, CancellationToken cancellationToken)
            {
                var _Cliente = await _repositoryAsync.ListAsync();
                var _ClienteDto = _mapper.Map<List<ClienteDto>>(_Cliente);
                return new Response<List<ClienteDto>>(_ClienteDto);
            }

           
            
        }

    }

    
}
