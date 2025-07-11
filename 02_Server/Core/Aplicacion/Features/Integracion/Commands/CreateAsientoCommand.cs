﻿using Aplicacion.DTOs.Integracion;
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

    public class CreateAsientoCommand : IRequest<Response<int>>
    {
        public AsientoDto Asiento { get; set; }
    }

    public class CreateAsientoCommandHandler : IRequestHandler<CreateAsientoCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Asiento> _repositoryAsync;
        private readonly IRepositoryAsync<Bus> _repositoryBus; // <-- Agrega esta línea
        private readonly IMapper _mapper;
        public CreateAsientoCommandHandler(
            IRepositoryAsync<Asiento> repositoryAsync, 
            IRepositoryAsync<Bus> repositoryBus, 
            IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _repositoryBus = repositoryBus;       // <-- Asigna el repositorio
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateAsientoCommand request, CancellationToken cancellationToken)
        {
            // 1. Obtener el bus para validar que exista y capacidad
            var bus = await _repositoryBus.GetByIdAsync(request.Asiento.IdBus);
            if (bus == null)
                throw new Exception("El bus especificado no existe.");

            // 2. Validar que el número de asiento no exceda la capacidad del bus
            if (request.Asiento.Numero > bus.Capacidad)
                throw new Exception($"El número de asiento ({request.Asiento.Numero}) no puede ser mayor a la capacidad del bus ({bus.Capacidad}).");

            // 3. Mapear y guardar el nuevo asiento
            var nuevoRegistro = _mapper.Map<Asiento>(request.Asiento);
            var data = await _repositoryAsync.AddAsync(nuevoRegistro);
            return new Response<int>(data.IdAsiento);
        }

       
    }

    
}
