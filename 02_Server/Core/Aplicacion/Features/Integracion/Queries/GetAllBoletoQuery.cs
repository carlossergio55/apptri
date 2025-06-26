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

    public class GetAllBoletoQuery : IRequest<Response<List<BoletoDto>>>
    {
        


        public class GetAllBoletoQueryHandler : IRequestHandler<GetAllBoletoQuery, Response<List<BoletoDto>>>
        {
            private readonly IRepositoryAsync<Boleto> _repositoryAsync;
            private readonly IMapper _mapper;
            public GetAllBoletoQueryHandler(IRepositoryAsync<Boleto> repositoryAsync, IMapper mapper)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
            }

            public async Task<Response<List<BoletoDto>>> Handle(GetAllBoletoQuery request, CancellationToken cancellationToken)
            {
                var _Boleto = await _repositoryAsync.ListAsync();
                var _BoletoDto = _mapper.Map<List<BoletoDto>>(_Boleto);
                return new Response<List<BoletoDto>>(_BoletoDto);
            }

          
        }

    }


}
