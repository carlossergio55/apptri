using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Integracion;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Commands.ViajeC
{
    public class ActualizarEstadosViajeCommand : IRequest<Response<int>>
    {

        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
    }

    public class ActualizarEstadosViajeCommandHandler
        : IRequestHandler<ActualizarEstadosViajeCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Viaje> _repoViaje;

        public ActualizarEstadosViajeCommandHandler(IRepositoryAsync<Viaje> repoViaje)
        {
            _repoViaje = repoViaje;
        }

        public async Task<Response<int>> Handle(ActualizarEstadosViajeCommand r, CancellationToken ct)
        {
            // Ventana por defecto (reduce carga)
            var hoy = DateTime.Today;
            var desde = (r.Desde ?? hoy.AddDays(-1)).Date;
            var hasta = (r.Hasta ?? hoy.AddDays(2)).Date;

            // Trae viajes en ventana que no estén cancelados ni finalizados muy antiguos
            var todos = await _repoViaje.ListAsync();
            var candidatos = todos.Where(v =>
                    v.Fecha.Date >= desde && v.Fecha.Date <= hasta &&
                    !string.Equals(v.Estado, "CANCELADO", StringComparison.OrdinalIgnoreCase))
                .ToList();

            var ahora = DateTime.Now;
            var modificados = 0;

            foreach (var v in candidatos)
            {
                var salida = v.Fecha.Date + v.HoraSalida; // HoraSalida es TimeSpan en tu Dominio
                var extendido = EsExtendido(v.Fecha, v.HoraSalida);
                var eta = salida + DuracionEstimadats(extendido); // ETA según extendido o no

                var nuevo = EstadoSegunTiempo(v.Estado, salida, eta, ahora);

                if (!string.Equals(nuevo, v.Estado, StringComparison.OrdinalIgnoreCase))
                {
                    v.Estado = nuevo;
                    await _repoViaje.UpdateAsync(v);
                    modificados++;
                }
            }

            return new Response<int>(modificados)
            {
                Message = $"Estados actualizados: {modificados} viaje(s)."
            };
        }


        private static bool EsExtendido(DateTime fecha, TimeSpan hora) =>
            (hora.Hours == 17 && hora.Minutes == 30) &&
            (fecha.DayOfWeek == DayOfWeek.Thursday || fecha.DayOfWeek == DayOfWeek.Sunday);

   
        private static TimeSpan DuracionEstimadats(bool extendido) =>
            extendido ? TimeSpan.FromHours(5) : TimeSpan.FromHours(4);

       
        private static string EstadoSegunTiempo(string estadoActual, DateTime salida, DateTime eta, DateTime ahora)
        {
            if (string.Equals(estadoActual, "CANCELADO", StringComparison.OrdinalIgnoreCase))
                return estadoActual;

            if (ahora >= eta) return "FINALIZADO";
            if (ahora >= salida) return "ENRUTA";
            if (ahora >= salida.AddMinutes(-30)) return "EMBARCANDO";

            return "PROGRAMADO";
        }
    }
}
