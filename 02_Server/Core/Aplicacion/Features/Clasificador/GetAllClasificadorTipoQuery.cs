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

namespace Aplicacion.Features.Clasificador
{

    public class GetAllGenClasificadortipoQuery : IRequest<Response<List<GenClasificadortipoDto>>>
    {



        public class GetAllGenClasificadortipoQueryHandler : IRequestHandler<GetAllGenClasificadortipoQuery, Response<List<GenClasificadortipoDto>>>
        {
            private readonly IRepositoryAsync<GenClasificadortipo> _repositoryAsync;
            private readonly IMapper _mapper;
            public GetAllGenClasificadortipoQueryHandler(IRepositoryAsync<GenClasificadortipo> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<Response<List<GenClasificadortipoDto>>> Handle(GetAllGenClasificadortipoQuery request, CancellationToken cancellationToken)
            {
                var _GenClasificadortipo = await _repositoryAsync.ListAsync(new GenClasificadortipoSpecification());
                var _GenClasificadortipoDto = _mapper.Map<List<GenClasificadortipoDto>>(_GenClasificadortipo);
                return new Response<List<GenClasificadortipoDto>>(_GenClasificadortipoDto);
            }

        }

    }
    public class GenClasificadortipoSpecification : Specification<GenClasificadortipo>
    {
        public GenClasificadortipoSpecification()
        {

        }
    }

}
