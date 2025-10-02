using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Entities.Integracion;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aplicacion.Features.Integracion.Queries
{
    public class GetViajesPlanillaQuery : IRequest<Response<List<ViajePlanillaDto>>>
    {
        public DateTime Fecha { get; set; }
        public int Dias { get; set; } = 1;
    }

    public class GetViajesPlanillaQueryHandler :
        IRequestHandler<GetViajesPlanillaQuery, Response<List<ViajePlanillaDto>>>
    {
        private readonly IRepositoryAsync<Viaje> _viajeRepo;
        private readonly IRepositoryAsync<Ruta> _rutaRepo;
        private readonly IRepositoryAsync<Bus> _busRepo;
        private readonly IRepositoryAsync<Chofer> _choferRepo;
        private readonly IRepositoryAsync<Asiento> _asientoRepo;
        private readonly IRepositoryAsync<Boleto> _boletoRepo;

        public GetViajesPlanillaQueryHandler(
            IRepositoryAsync<Viaje> viajeRepo,
            IRepositoryAsync<Ruta> rutaRepo,
            IRepositoryAsync<Bus> busRepo,
            IRepositoryAsync<Chofer> choferRepo,
            IRepositoryAsync<Asiento> asientoRepo,
            IRepositoryAsync<Boleto> boletoRepo)
        {
            _viajeRepo = viajeRepo;
            _rutaRepo = rutaRepo;
            _busRepo = busRepo;
            _choferRepo = choferRepo;
            _asientoRepo = asientoRepo;
            _boletoRepo = boletoRepo;
        }

        public async Task<Response<List<ViajePlanillaDto>>> Handle(GetViajesPlanillaQuery request, CancellationToken ct)
        {
            var desde = request.Fecha.Date;
            var hasta = desde.AddDays(request.Dias);

            // 🔹 Filtramos solo los viajes del rango
            var viajes = (await _viajeRepo.ListAsync(ct))
     .Where(v => v.Fecha >= desde && v.Fecha < hasta)
     .ToList();


            var rutas = await _rutaRepo.ListAsync(ct);
            var buses = await _busRepo.ListAsync(ct);
            var choferes = await _choferRepo.ListAsync(ct);
            var asientos = await _asientoRepo.ListAsync(ct);
            var boletos = await _boletoRepo.ListAsync(ct);

            var result = viajes.Select(v =>
            {
                var bus = buses.FirstOrDefault(b => b.IdBus == v.IdBus);
                var ruta = rutas.FirstOrDefault(r => r.IdRuta == v.IdRuta);
                var chofer = choferes.FirstOrDefault(c => c.IdChofer == v.IdChofer);

                var cap = asientos.Count(a => a.IdBus == v.IdBus);
                var ocupados = boletos.Count(b => b.IdViaje == v.IdViaje && b.Estado == "CONFIRMADO");

                return new ViajePlanillaDto
                {
                    IdViaje = v.IdViaje,
                    Fecha = v.Fecha,
                    HoraSalida = (v.HoraSalida is TimeSpan ts)
                        ? ts.ToString(@"hh\:mm") // Si HoraSalida es TimeSpan
                        : v.HoraSalida.ToString("HH:mm"), // Si es DateTime?
                    Estado = v.Estado ?? "PROGRAMADO",
                    RutaNombre = ruta != null ? $"{ruta.Origen} - {ruta.Destino}" : "",
                    Placa = bus?.Placa ?? "",
                    ChoferNombre = chofer?.Nombre ?? "",
                    Capacidad = cap,
                    Ocupados = ocupados
                };
            }).ToList();

            return new Response<List<ViajePlanillaDto>>(result);
        }
    }
}
