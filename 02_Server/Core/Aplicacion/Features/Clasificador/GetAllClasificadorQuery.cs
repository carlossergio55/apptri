using Aplicacion.DTOs.Clasificador;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Ardalis.Specification;
using AutoMapper;
using Dominio.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Aplicacion.Clasificador.Queries
{

    public class GetAllClasificadorQuery : IRequest<Response<List<GenClasificadorDto>>>
    {

        public class GetAllClasificadorQueryHandler : IRequestHandler<GetAllClasificadorQuery, Response<List<GenClasificadorDto>>>
        {
            private readonly IRepositoryAsync<GenClasificador> _repositoryAsync;
            private readonly IMapper _mapper;
            public GetAllClasificadorQueryHandler(IRepositoryAsync<GenClasificador> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<Response<List<GenClasificadorDto>>> Handle(GetAllClasificadorQuery request, CancellationToken cancellationToken)
            {
                var _Clasificador = await _repositoryAsync.ListAsync(new ClasificadorSpecification(), cancellationToken);
                var _ClasificadorDto = _mapper.Map<List<GenClasificadorDto>>(_Clasificador);
                return new Response<List<GenClasificadorDto>>(_ClasificadorDto);
            }
        }

    }

    public class ClasificadorSpecification : Specification<GenClasificador>
    {
        public ClasificadorSpecification()
        {

        }
    }
}
