using Aplicacion.Interfaces;
using Dominio.Entities.Integracion;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Queries
{
    // VM simple para la respuesta
    public class ParadaVm
    {
        public int IdParada { get; set; }
        public string Nombre { get; set; } = "";
        public int Orden { get; set; }
    }

    // Query
    public class GetParadasDeViajeQuery : IRequest<List<ParadaVm>>
    {
        public int IdViaje { get; set; }
    }

    // Handler
    public class GetParadasDeViajeQueryHandler
        : IRequestHandler<GetParadasDeViajeQuery, List<ParadaVm>>
    {
        private readonly IRepositoryAsync<Viaje> _repoViaje;

        public GetParadasDeViajeQueryHandler(IRepositoryAsync<Viaje> repoViaje)
        {
            _repoViaje = repoViaje;
        }

        public async Task<List<ParadaVm>> Handle(GetParadasDeViajeQuery req, CancellationToken ct)
        {
            var v = await _repoViaje.GetByIdAsync(req.IdViaje);
            if (v is null) return new();

            // Regla: Jueves/Domingo a las 17:30 => extendido
            bool esExtendido =
                (v.HoraSalida.Hours == 17 && v.HoraSalida.Minutes == 30) &&
                (v.Fecha.DayOfWeek == DayOfWeek.Thursday || v.Fecha.DayOfWeek == DayOfWeek.Sunday);

            // ⚠️ AJUSTA estos IdParada a los reales de tu tabla
            var paradas = new List<ParadaVm>
            {
                new() { IdParada = 4, Nombre = "Sucre",          Orden = 0 },
                new() { IdParada = 1, Nombre = "Tomina",         Orden = 1 },
                new() { IdParada = 2, Nombre = "Villa Serrano",  Orden = 2 }
            };

            if (esExtendido)
                paradas.Add(new ParadaVm { IdParada = 3, Nombre = "Mendoza", Orden = paradas.Count });

            return paradas;
        }
    }
}
