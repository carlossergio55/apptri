using Aplicacion.DTOs.Integracion;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Ardalis.Specification;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Queries
{

    public class GetAllAsientoQuery : IRequest<Response<List<AsientoDto>>>
    {



        public class GetAllAsientoQueryHandler : IRequestHandler<GetAllAsientoQuery, Response<List<AsientoDto>>>
        {
            private readonly IRepositoryAsync<Asiento> _repositoryAsync;
            private readonly IMapper _mapper;
            public GetAllAsientoQueryHandler(IRepositoryAsync<Asiento> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<Response<List<AsientoDto>>> Handle(GetAllAsientoQuery request, CancellationToken cancellationToken)
            {
                var _Asiento = await _repositoryAsync.ListAsync(new AsientoSpecification(), cancellationToken);
                var _AsientoDto = _mapper.Map<List<AsientoDto>>(_Asiento);
                return new Response<List<AsientoDto>>(_AsientoDto);
            }

        }
        public class AsientoSpecification : Specification<Asiento>
        {
            public AsientoSpecification()
            {
                Query.Include(x => x.Bus);
                
            }
        }

    }
}

