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
        public int IdBusDefault { get; set; } = 1;
        public int IdChoferDefault { get; set; } = 1;
    }

    public record GenerarViajesResult(
        List<int> Creados,
        List<(DateTime Fecha, int IdRuta, string Hora, string Motivo)> Omitidos,
        List<(DateTime Fecha, int IdRuta, string Hora, string Error)> Errores,
        int TotalProcesados);

    public class GenerarViajesProximosCommandHandler
        : IRequestHandler<GenerarViajesProximosCommand, Response<GenerarViajesResult>>
    {
        private readonly IRepositoryAsync<Viaje> _repoViaje;
        private readonly IRepositoryAsync<Chofer> _repoChofer;
        private readonly IRepositoryAsync<Bus> _repoBus;

        public GenerarViajesProximosCommandHandler(
            IRepositoryAsync<Viaje> repoViaje,
            IRepositoryAsync<Chofer> repoChofer,
            IRepositoryAsync<Bus> repoBus)
        {
            _repoViaje = repoViaje;
            _repoChofer = repoChofer;
            _repoBus = repoBus;
        }

        public async Task<Response<GenerarViajesResult>> Handle(GenerarViajesProximosCommand r, CancellationToken ct)
        {
            var creados = new List<int>();
            var omitidos = new List<(DateTime, int, string, string)>();
            var errores = new List<(DateTime, int, string, string)>();

            const int RUTA_SERRANO = 1; // única ruta

            var h0900 = TimeSpan.ParseExact("09:00", @"hh\:mm", CultureInfo.InvariantCulture);
            var h1730 = TimeSpan.ParseExact("17:30", @"hh\:mm", CultureInfo.InvariantCulture);

            var choferes = (await _repoChofer.ListAsync(ct)).Select(c => c.IdChofer).ToList();
            var buses = (await _repoBus.ListAsync(ct)).Select(b => b.IdBus).ToList();

            if (!choferes.Any() || !buses.Any())
                return new Response<GenerarViajesResult>(new GenerarViajesResult(creados, omitidos, errores, 0))
                { Message = "No hay choferes o buses en BD.", Succeeded = false };

            // parejas Chofer-Bus
            var parejas = new List<(int ChoferId, int BusId)>();
            var max = Math.Max(choferes.Count, buses.Count);
            for (int i = 0; i < max; i++)
                parejas.Add((choferes[i % choferes.Count], buses[i % buses.Count]));
            var idxPareja = 0;

            var start = r.Desde.Date;
            var end = start.AddDays(r.Dias);
            var todos = await _repoViaje.ListAsync(ct);

            var usadosPorSlot = new Dictionary<(DateTime Fecha, TimeSpan Hora), (HashSet<int> Buses, HashSet<int> Choferes)>();

            for (var d = start; d < end; d = d.AddDays(1))
            {
                var slots = new List<(TimeSpan Hora, string Dir)>
                {
                    (h0900, "IDA"),
                    (h1730, "IDA")
                };

                foreach (var (hora, dir) in slots)
                    await TryCreate(d, RUTA_SERRANO, hora, dir);
            }

            var result = new GenerarViajesResult(
                Creados: creados,
                Omitidos: omitidos,
                Errores: errores,
                TotalProcesados: creados.Count + omitidos.Count + errores.Count
            );

            return new Response<GenerarViajesResult>(result)
            {
                Message = $"Generación completada. Creados: {creados.Count}, Omitidos: {omitidos.Count}, Errores: {errores.Count}"
            };

            async Task TryCreate(DateTime fecha, int idRuta, TimeSpan hora, string dir)
            {
                if (todos.Any(v => v.IdRuta == idRuta && v.Fecha.Date == fecha.Date && v.HoraSalida == hora))
                {
                    omitidos.Add((fecha, idRuta, hora.ToString(@"hh\:mm"), "Ya existe"));
                    return;
                }

                var key = (fecha.Date, hora);
                if (!usadosPorSlot.TryGetValue(key, out var usados))
                {
                    usados = (new HashSet<int>(), new HashSet<int>());
                    usadosPorSlot[key] = usados;
                }

                (int ChoferId, int BusId) parejaElegida = default;
                bool asignada = false;

                // Buscar pareja disponible
                for (int t = 0; t < parejas.Count; t++)
                {
                    var p = parejas[(idxPareja + t) % parejas.Count];

                    bool choferLibreSlot = !usados.Choferes.Contains(p.ChoferId);
                    bool busLibreSlot = !usados.Buses.Contains(p.BusId);
                    bool choferLibreBD = !todos.Any(v => v.Fecha.Date == fecha.Date && v.HoraSalida == hora && v.IdChofer == p.ChoferId);
                    bool busLibreBD = !todos.Any(v => v.Fecha.Date == fecha.Date && v.HoraSalida == hora && v.IdBus == p.BusId);

                    if (choferLibreSlot && busLibreSlot && choferLibreBD && busLibreBD)
                    {
                        parejaElegida = p;
                        idxPareja = (idxPareja + t + 1) % parejas.Count;
                        usados.Choferes.Add(p.ChoferId);
                        usados.Buses.Add(p.BusId);
                        asignada = true;
                        break;
                    }
                }

                if (!asignada)
                {
                    var p = (ChoferId: r.IdChoferDefault, BusId: r.IdBusDefault);

                    bool choferLibreSlot = !usados.Choferes.Contains(p.ChoferId);
                    bool busLibreSlot = !usados.Buses.Contains(p.BusId);
                    bool choferLibreBD = !todos.Any(v => v.Fecha.Date == fecha.Date && v.HoraSalida == hora && v.IdChofer == p.ChoferId);
                    bool busLibreBD = !todos.Any(v => v.Fecha.Date == fecha.Date && v.HoraSalida == hora && v.IdBus == p.BusId);

                    if (choferLibreSlot && busLibreSlot && choferLibreBD && busLibreBD)
                    {
                        parejaElegida = p;
                        usados.Choferes.Add(p.ChoferId);
                        usados.Buses.Add(p.BusId);
                        asignada = true;
                    }
                }

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

                try
                {
                    var saved = await _repoViaje.AddAsync(entity, ct);
                    creados.Add(saved.IdViaje);
                    todos.Add(saved);
                }
                catch (Exception ex)
                {
                    errores.Add((fecha, idRuta, hora.ToString(@"hh\:mm"), ex.Message));
                }
            }
        }
    }
}
