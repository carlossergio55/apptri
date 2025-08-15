using Aplicacion.DTOs.Integracion;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using AutoMapper;
using Dominio.Entities.Integracion;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.HorarioC
{
    public class CreateHorarioCommand : IRequest<Response<int>>
    {
        public HorarioDto Horario { get; set; } = null!;
    }

    public class CreateHorarioCommandHandler : IRequestHandler<CreateHorarioCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Horario> _repo;
        private readonly IRepositoryAsync<Ruta> _repoRuta;
        private readonly IRepositoryAsync<RutaParada> _repoRutaParada;
        private readonly IMapper _mapper;

        public CreateHorarioCommandHandler(
            IRepositoryAsync<Horario> repo,
            IRepositoryAsync<Ruta> repoRuta,
            IRepositoryAsync<RutaParada> repoRutaParada,
            IMapper mapper)
        {
            _repo = repo;
            _repoRuta = repoRuta;
            _repoRutaParada = repoRutaParada;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateHorarioCommand request, CancellationToken ct)
        {
            var dto = request.Horario;

            // 1) Ruta debe existir
            _ = await _repoRuta.GetByIdAsync(dto.IdRuta)
                ?? throw new Exception("La ruta especificada no existe.");

            // 2) Validar tramo (si viene)
            int? ordenDesde = null, ordenHasta = null;
            if (dto.DesdeParadaId.HasValue || dto.HastaParadaId.HasValue)
            {
                if (!dto.DesdeParadaId.HasValue || !dto.HastaParadaId.HasValue)
                    throw new Exception("Debe especificar 'DesdeParadaId' y 'HastaParadaId' juntos.");

                var paradasRuta = (await _repoRutaParada.ListAsync())
                                  .Where(x => x.IdRuta == dto.IdRuta)
                                  .ToList();

                var desde = paradasRuta.FirstOrDefault(p => p.IdParada == dto.DesdeParadaId);
                var hasta = paradasRuta.FirstOrDefault(p => p.IdParada == dto.HastaParadaId);

                if (desde is null || hasta is null)
                    throw new Exception("Las paradas no pertenecen a la ruta.");

                ordenDesde = desde.Orden;
                ordenHasta = hasta.Orden;

                var dir = (dto.Direccion ?? "IDA").ToUpperInvariant();
                if (dir == "IDA" && ordenDesde >= ordenHasta)
                    throw new Exception("Para IDA el orden debe ser ascendente (desde < hasta).");
                if (dir == "VUELTA" && ordenDesde <= ordenHasta)
                    throw new Exception("Para VUELTA el orden debe ser descendente (desde > hasta).");
            }

            // 3) Evitar duplicado exacto de horario
            var existentes = await _repo.ListAsync();
            bool duplicado = existentes.Any(h =>
                h.IdRuta == dto.IdRuta &&
                (h.Direccion ?? "IDA").ToUpper() == (dto.Direccion ?? "IDA").ToUpper() &&
                h.DiaSemana == dto.DiaSemana &&
                h.HoraSalida == dto.HoraSalida &&
                h.DesdeParadaId == dto.DesdeParadaId &&
                h.HastaParadaId == dto.HastaParadaId
            );
            if (duplicado)
                throw new Exception("Ya existe un horario con los mismos datos.");

            // 4) Crear
            var entity = _mapper.Map<Horario>(dto);
            var saved = await _repo.AddAsync(entity);
            return new Response<int>(saved.IdHorario);
        }
    }
}
