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

    public class GetAllUsuarioQuery : IRequest<Response<List<UsuarioDto>>>
    {
     


        public class GetAllUsuarioQueryHandler : IRequestHandler<GetAllUsuarioQuery, Response<List<UsuarioDto>>>
        {
            private readonly IRepositoryAsync<Usuario> _repositoryAsync;
            private readonly IMapper _mapper;
            public GetAllUsuarioQueryHandler(IRepositoryAsync<Usuario> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<Response<List<UsuarioDto>>> Handle(GetAllUsuarioQuery request, CancellationToken cancellationToken)
            {
                var _Usuario = await _repositoryAsync.ListAsync();
                var _UsuarioDto = _mapper.Map<List<UsuarioDto>>(_Usuario);
                return new Response<List<UsuarioDto>>(_UsuarioDto);
            }

            
        }

    }

    
    
}
