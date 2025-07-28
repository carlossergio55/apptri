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

namespace Aplicacion.Features.Integracion.Commands.BoletoC
{

    public class CreateBoletoCommand : IRequest<Response<int>>
    {
        public BoletoDto Boleto { get; set; }
    }

    public class CreateBoletoCommandHandler : IRequestHandler<CreateBoletoCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Boleto> _repositoryAsync;
        private readonly IMapper _mapper;
        public CreateBoletoCommandHandler(IRepositoryAsync<Boleto> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateBoletoCommand request, CancellationToken cancellationToken)
        {
            var nuevoRegistro = _mapper.Map<Boleto>(request.Boleto);
            var data = await _repositoryAsync.AddAsync(nuevoRegistro);
            return new Response<int>(data.IdBoleto);
        }


    }
}
  
