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

namespace Aplicacion.Features.Integracion.Commands.AsientoC
{

    public class CreateAsientoCommand : IRequest<Response<int>>
    {
        public AsientoDto Asiento { get; set; }
    }

    public class CreateAsientoCommandHandler : IRequestHandler<CreateAsientoCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Asiento> _repositoryAsync;
        private readonly IRepositoryAsync<Bus> _repositoryBus; 
        private readonly IMapper _mapper;
        public CreateAsientoCommandHandler(
            IRepositoryAsync<Asiento> repositoryAsync, 
            IRepositoryAsync<Bus> repositoryBus, 
            IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _repositoryBus = repositoryBus;       
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateAsientoCommand request, CancellationToken cancellationToken)
        {
            
            var bus = await _repositoryBus.GetByIdAsync(request.Asiento.IdBus);
            if (bus == null)
                throw new Exception("El bus especificado no existe.");

            
            if (request.Asiento.Numero > bus.Capacidad)
                throw new Exception($"El número de asiento ({request.Asiento.Numero}) no puede ser mayor a la capacidad del bus ({bus.Capacidad}).");

            
            var nuevoRegistro = _mapper.Map<Asiento>(request.Asiento);
            var data = await _repositoryAsync.AddAsync(nuevoRegistro);
            return new Response<int>(data.IdAsiento);
        }

       
    }

    
}
