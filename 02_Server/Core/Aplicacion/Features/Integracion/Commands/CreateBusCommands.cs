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

namespace Aplicacion.Features.Integracion.Commands
{

    public class CreateBusCommand : IRequest<Response<int>>
    {
        public BusDto Bus { get; set; }
    }

    public class CreateBusCommandHandler : IRequestHandler<CreateBusCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Bus> _repositoryAsync;
        private readonly IMapper _mapper;
        public CreateBusCommandHandler(IRepositoryAsync<Bus> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateBusCommand request, CancellationToken cancellationToken)
        {
            var nuevoRegistro = _mapper.Map<Bus>(request.Bus);
            var data = await _repositoryAsync.AddAsync(nuevoRegistro);
            return new Response<int>(data.IdBus);
        }

     
    }

      
    
}
