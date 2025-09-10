using Aplicacion.Wrappers;
using Dominio.Entities.Integracion;
using Aplicacion.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.ViajeC
{
    public class GenerarViajesProximosCommand : IRequest<Response<GenerarViajesResult>>
    {
        public DateTime Desde { get; set; }
        public int Dias { get; set; } = 14;

        // Fallback si no hay pareja libre (no debería usarse normalmente)
        public int IdBusDefault { get; set; } = 1;
        public int IdChoferDefault { get; set; } = 1;
    }

    public record GenerarViajesResult(
        List<int> Creados,
        List<(DateTime Fecha, int IdRuta, string Hora)> Existentes,
        int TotalProcesados);

    public class GenerarViajesProximosCommandHandler
        : IRequestHandler<GenerarViajesProximosCommand, Response<GenerarViajesResult>>
    {
        private readonly IRepositoryAsync<Viaje> _repoViaje;

        public GenerarViajesProximosCommandHandler(IRepositoryAsync<Viaje> repoViaje)
        {
            _repoViaje = repoViaje;
        }

        public async Task<Response<GenerarViajesResult>> Handle(GenerarViajesProximosCommand r, CancellationToken ct)
        {
            var creados = new List<int>();
            var existentes = new List<(DateTime, int, string)>();

            // Rutas
            const int RUTA_SERRANO = 1;
            const int RUTA_MENDOZA = 5;

            // Horas
            var h0900 = TimeSpan.ParseExact("09:00", @"hh\:mm", CultureInfo.InvariantCulture);
            var h1730 = TimeSpan.ParseExact("17:30", @"hh\:mm", CultureInfo.InvariantCulture);

            // Rotación fija (Chofer, Bus)
            var parejas = new List<(int ChoferId, int BusId)>
            {
                (1, 1),
                (4, 5),
                (3, 2)
            };
            var idxPareja = 0;

            var start = r.Desde.Date;
            var end = start.AddDays(r.Dias); // exclusivo

            // Cache de viajes ya existentes en el rango (para evitar duplicados y choques)
            var todos = await _repoViaje.ListAsync();

            // Para no asignar el mismo chofer/bus en el MISMO horario (ej: dos viajes 17:30 el mismo día)
            // clave: (Fecha, Hora) -> usados en ese slot
            var usadosPorSlot = new Dictionary<(DateTime Fecha, TimeSpan Hora), (HashSet<int> Buses, HashSet<int> Choferes)>();

            for (var d = start; d < end; d = d.AddDays(1))
            {
                // slots del día
                var slots = new List<(int RutaId, TimeSpan Hora, string Dir)>
                {
                    (RUTA_SERRANO, h0900, "IDA"),
                    (RUTA_SERRANO, h1730, "IDA")
                };
                if (d.DayOfWeek == DayOfWeek.Thursday || d.DayOfWeek == DayOfWeek.Sunday)
                    slots.Add((RUTA_MENDOZA, h1730, "IDA"));

                foreach (var (rutaId, hora, dir) in slots)
                {
                    await TryCreate(d, rutaId, hora, dir);
                }
            }

            var result = new GenerarViajesResult(
                Creados: creados,
                Existentes: existentes,
                TotalProcesados: creados.Count + existentes.Count
            );

            return new Response<GenerarViajesResult>(result)
            {
                Message = "Generación completada."
            };

            // --------- helpers internos ---------

            async Task TryCreate(DateTime fecha, int idRuta, TimeSpan hora, string dir)
            {
                // ¿Ya existe ese viaje exacto?
                var yaExiste = todos.Any(v =>
                    v.IdRuta == idRuta &&
                    v.Fecha.Date == fecha.Date &&
                    v.HoraSalida == hora);

                if (yaExiste)
                {
                    existentes.Add((fecha, idRuta, hora.ToString(@"hh\:mm")));
                    return;
                }

                // Slot key para controlar chofer/bus en el mismo horario
                var key = (fecha.Date, hora);
                if (!usadosPorSlot.TryGetValue(key, out var usados))
                {
                    usados = (new HashSet<int>(), new HashSet<int>());
                    usadosPorSlot[key] = usados;
                }

                // Buscar pareja libre (no usada en este slot, ni por "todos" ya existentes en este mismo horario)
                (int ChoferId, int BusId) parejaElegida = default;
                bool asignada = false;

                for (int t = 0; t < parejas.Count; t++)
                {
                    var p = parejas[(idxPareja + t) % parejas.Count];

                    bool choferLibreEnSlot = !usados.Choferes.Contains(p.ChoferId);
                    bool busLibreEnSlot = !usados.Buses.Contains(p.BusId);

                    bool choferLibreEnBD = !todos.Any(v =>
                        v.Fecha.Date == fecha.Date &&
                        v.HoraSalida == hora &&
                        v.IdChofer == p.ChoferId);

                    bool busLibreEnBD = !todos.Any(v =>
                        v.Fecha.Date == fecha.Date &&
                        v.HoraSalida == hora &&
                        v.IdBus == p.BusId);

                    if (choferLibreEnSlot && busLibreEnSlot && choferLibreEnBD && busLibreEnBD)
                    {
                        parejaElegida = p;
                        idxPareja = (idxPareja + t + 1) % parejas.Count; // avanzar puntero de rotación
                        usados.Choferes.Add(p.ChoferId);
                        usados.Buses.Add(p.BusId);
                        asignada = true;
                        break;
                    }
                }

                // Fallback (si no hay pareja libre, usa los defaults si no chocan en el slot)
                if (!asignada)
                {
                    var p = (ChoferId: r.IdChoferDefault, BusId: r.IdBusDefault);

                    bool choferLibreEnSlot = !usados.Choferes.Contains(p.ChoferId);
                    bool busLibreEnSlot = !usados.Buses.Contains(p.BusId);

                    bool choferLibreEnBD = !todos.Any(v =>
                        v.Fecha.Date == fecha.Date && v.HoraSalida == hora && v.IdChofer == p.ChoferId);

                    bool busLibreEnBD = !todos.Any(v =>
                        v.Fecha.Date == fecha.Date && v.HoraSalida == hora && v.IdBus == p.BusId);

                    if (choferLibreEnSlot && busLibreEnSlot && choferLibreEnBD && busLibreEnBD)
                    {
                        parejaElegida = p;
                        usados.Choferes.Add(p.ChoferId);
                        usados.Buses.Add(p.BusId);
                        asignada = true;
                    }
                }

                // Si aún no asignamos, como última opción toma la siguiente pareja de rotación (podría chocar si no hay recursos)
                if (!asignada)
                {
                    var p = parejas[idxPareja];
                    parejaElegida = p;
                    idxPareja = (idxPareja + 1) % parejas.Count;
                    usados.Choferes.Add(p.ChoferId);
                    usados.Buses.Add(p.BusId);
                }

                var entity = new Viaje
                {
                    Fecha = fecha.Date,
                    HoraSalida = hora,
                    Estado = "PROGRAMADO",
                    Direccion = dir,
                    IdRuta = idRuta,
                    IdBus = parejaElegida.BusId,
                    IdChofer = parejaElegida.ChoferId
                };

                var saved = await _repoViaje.AddAsync(entity);
                creados.Add(saved.IdViaje);

                // Añadir al cache local para próximos chequeos dentro del mismo proceso
                todos.Add(saved);
            }
        }
    }
}
