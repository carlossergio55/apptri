using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.ViajeC
{
    public class UpdateViajeCommand : IRequest<Response<int>>
    {
        public int IdViaje { get; set; }
        public DateTime Fecha { get; set; }
        public string HoraSalida { get; set; }
        public string Estado { get; set; }
        public int IdRuta { get; set; }
        public int IdChofer { get; set; }
        public int IdBus { get; set; }
    }

    public class UpdateViajeCommandHandler : IRequestHandler<UpdateViajeCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Viaje> _repositoryAsync;
        private readonly IMapper _mapper;

        public UpdateViajeCommandHandler(IRepositoryAsync<Viaje> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(UpdateViajeCommand request, CancellationToken cancellationToken)
        {
            var viaje = await _repositoryAsync.GetByIdAsync(request.IdViaje);
            if (viaje == null)
                throw new KeyNotFoundException("Registro no encontrado");

            viaje.Fecha = request.Fecha;
            viaje.HoraSalida = TimeSpan.Parse(request.HoraSalida); // Asegúrate de que el campo en tu entidad es TimeSpan
            viaje.Estado = request.Estado;
            viaje.IdRuta = request.IdRuta;
            viaje.IdChofer = request.IdChofer;
            viaje.IdBus = request.IdBus;

            await _repositoryAsync.UpdateAsync(viaje);
            return new Response<int>(viaje.IdViaje);
        }
    }
}
