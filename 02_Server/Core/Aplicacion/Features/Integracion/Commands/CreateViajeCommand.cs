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

    public class CreateViajeCommand : IRequest<Response<int>>
    {
        public ViajeDto Viaje { get; set; }
    }

    public class CreateViajeCommandHandler : IRequestHandler<CreateViajeCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Viaje> _repositoryAsync;
        private readonly IRepositoryAsync<Ruta> _repositoryRuta;
        private readonly IRepositoryAsync<Chofer> _repositoryChofer;
        private readonly IRepositoryAsync<Bus> _repositoryBus;
        private readonly IMapper _mapper;

        public CreateViajeCommandHandler(
            IRepositoryAsync<Viaje> repositoryAsync,
            IRepositoryAsync<Ruta> repositoryRuta,
            IRepositoryAsync<Chofer> repositoryChofer,
            IRepositoryAsync<Bus> repositoryBus,
            IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _repositoryRuta = repositoryRuta;
            _repositoryChofer = repositoryChofer;
            _repositoryBus = repositoryBus;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateViajeCommand request, CancellationToken cancellationToken)
        {
            // Validar claves foráneas
            var ruta = await _repositoryRuta.GetByIdAsync(request.Viaje.IdRuta);
            if (ruta == null)
                throw new Exception("La ruta especificada no existe.");

            var chofer = await _repositoryChofer.GetByIdAsync(request.Viaje.IdChofer);
            if (chofer == null)
                throw new Exception("El chofer especificado no existe.");

            var bus = await _repositoryBus.GetByIdAsync(request.Viaje.IdBus);
            if (bus == null)
                throw new Exception("El bus especificado no existe.");

            // Mapear y guardar
            var nuevoRegistro = _mapper.Map<Viaje>(request.Viaje);
            var fechaHoy = DateTime.Today;
            var fechaLimite = fechaHoy.AddDays(7);
            // ✅ Convertir string a TimeSpan
            if (TimeSpan.TryParse(request.Viaje.HoraSalida, out TimeSpan hora))
            {
                nuevoRegistro.HoraSalida = hora;
            }
            else
            {
                throw new Exception("Formato inválido para la hora. Usa HH:mm o HH:mm:ss");
            }
            if (request.Viaje.Fecha.Date < fechaHoy)
                throw new Exception("La fecha del viaje no puede ser en el pasado.");

            if (request.Viaje.Fecha.Date > fechaLimite)
                throw new Exception("No se pueden programar viajes con más de una semana de anticipación.");

            // Guardar en la base de datos
            var data = await _repositoryAsync.AddAsync(nuevoRegistro);
            return new Response<int>(data.IdViaje);
        }


    }

}
