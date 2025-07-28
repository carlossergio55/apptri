using Aplicacion.DTOs.Integracion;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.AsientoC
{
    public class UpdateAsientoCommand : IRequest<Response<int>>
    {
        public int IdAsiento { get; set; }
        public int IdBus { get; set; }
        public int Numero { get; set; }
    }

    public class UpdateAsientoCommandHandler : IRequestHandler<UpdateAsientoCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Asiento> _repositoryAsync;
        private readonly IMapper _mapper;

        public UpdateAsientoCommandHandler(IRepositoryAsync<Asiento> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(UpdateAsientoCommand request, CancellationToken cancellationToken)
        {
            var asiento = await _repositoryAsync.GetByIdAsync(request.IdAsiento);
            if (asiento == null)
                throw new KeyNotFoundException("Registro no encontrado");

            // Actualizar los campos
            asiento.IdBus = request.IdBus;
            asiento.Numero = request.Numero;

            await _repositoryAsync.UpdateAsync(asiento);
            return new Response<int>(asiento.IdAsiento);
        }
    }
}
