using Aplicacion.DTOs.Integracion;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.ViajeC
{
    public class CreateViajeCommand : IRequest<Response<int>>
    {
        public ViajeDto Viaje { get; set; } = null!;
    }

    public class CreateViajeCommandHandler : IRequestHandler<CreateViajeCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Viaje> _viajes;
        private readonly IMapper _mapper;

        public CreateViajeCommandHandler(
            IRepositoryAsync<Viaje> viajes,
            IMapper mapper)
        {
            _viajes = viajes;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateViajeCommand request, CancellationToken ct)
        {
            var dto = request.Viaje;

            // Map base
            var entity = _mapper.Map<Viaje>(dto);

            // 1) Parsear HoraSalida (string -> TimeSpan)
            //    Acepta "HH:mm" o "HH:mm:ss"
            if (TimeSpan.TryParse(dto.HoraSalida, out var ts))
                entity.HoraSalida = ts;
            else
                throw new Exception("Formato inválido para HoraSalida. Usa HH:mm o HH:mm:ss.");

            // 2) Normalizar Dirección (opcional)
            entity.Direccion = string.IsNullOrWhiteSpace(dto.Direccion)
                ? "IDA"
                : dto.Direccion.Trim().ToUpperInvariant(); // "IDA" | "VUELTA"

            // 3) Tramo (opcional)
            entity.DesdeParadaId = dto.DesdeParadaId;
            entity.HastaParadaId = dto.HastaParadaId;

            // Guardar
            var saved = await _viajes.AddAsync(entity);
            return new Response<int>(saved.IdViaje);
        }
    }
}
