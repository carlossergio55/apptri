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

    public class GetAllPagoQuery : IRequest<Response<List<PagoDto>>>
    {
    


        public class GetAllPagoQueryHandler : IRequestHandler<GetAllPagoQuery, Response<List<PagoDto>>>
        {
            private readonly IRepositoryAsync<Pago> _repositoryAsync;
            private readonly IMapper _mapper;
            public GetAllPagoQueryHandler(IRepositoryAsync<Pago> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<Response<List<PagoDto>>> Handle(GetAllPagoQuery request, CancellationToken cancellationToken)
            {
                var _Pago = await _repositoryAsync.ListAsync();
                var _PagoDto = _mapper.Map<List<PagoDto>>(_Pago);
                return new Response<List<PagoDto>>(_PagoDto);
            }

        }

    }

  
}
