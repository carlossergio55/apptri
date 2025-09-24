using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using MediatR;

using Aplicacion.Wrappers;
using Aplicacion.Interfaces;                 // IRepositoryAsync<T>
using Dominio.Entities.Integracion;         // Boleto, Viaje

namespace Aplicacion.Features.Integracion.Commands.BoletoC
{
    // ===== Request =====
    public class ExpirarReservasVencidasCommand : IRequest<Response<ExpirarReservasResultDto>>
    {
        /// <summary>TTL en minutos para una reserva BLOQUEADO.</summary>
        public int ReservaTtlMinutos { get; set; } = 10;

        /// <summary>Horas antes de la salida del viaje a partir de las cuales toda reserva se vence.</summary>
        public int VentanaVencimientoHorasAntes { get; set; } = 2;
    }

    // ===== Resultado =====
    public class ExpirarReservasResultDto
    {
        public int TotalRevisadas { get; set; }
        public int TotalExpiradas { get; set; }
        public int ExpiradasPorTtl { get; set; }
        public int ExpiradasPorVentana { get; set; }
        public List<int> ExpiradasIds { get; set; } = new();
    }

    // ===== Handler =====
    public class ExpirarReservasVencidasCommandHandler
        : IRequestHandler<ExpirarReservasVencidasCommand, Response<ExpirarReservasResultDto>>
    {
        private readonly IRepositoryAsync<Boleto> _boletoRepo;
        private readonly IRepositoryAsync<Viaje> _viajeRepo;

        public ExpirarReservasVencidasCommandHandler(
            IRepositoryAsync<Boleto> boletoRepo,
            IRepositoryAsync<Viaje> viajeRepo)
        {
            _boletoRepo = boletoRepo;
            _viajeRepo = viajeRepo;
        }

        public async Task<Response<ExpirarReservasResultDto>> Handle(
            ExpirarReservasVencidasCommand request,
            CancellationToken ct)
        {
            var now = DateTime.Now; // **hora local** para ser consistente con Reservar/Confirmar
            var ttlMin = request.ReservaTtlMinutos;
            var ventanaHoras = request.VentanaVencimientoHorasAntes;

            // 1) Buscar boletos BLOQUEADO
            var todos = await _boletoRepo.ListAsync();
            var bloqueados = todos
                .Where(b => string.Equals(b.Estado ?? "", "BLOQUEADO", StringComparison.OrdinalIgnoreCase))
                .ToList();

            var result = new ExpirarReservasResultDto
            {
                TotalRevisadas = bloqueados.Count
            };

            if (bloqueados.Count == 0)
                return new Response<ExpirarReservasResultDto>(result, "No hay reservas bloqueadas.");

            // 2) Cargar viajes necesarios en un diccionario
            var viajeIds = bloqueados.Select(b => b.IdViaje).Distinct().ToList();
            var viajes = (await _viajeRepo.ListAsync())
                .Where(v => viajeIds.Contains(v.IdViaje))
                .ToDictionary(v => v.IdViaje, v => v);

            // 3) Evaluar vencimiento
            foreach (var b in bloqueados)
            {
                // TTL basado en FechaReservaUtc; si no hay, cae a FechaCompra; si tampoco hay, no expira por TTL
                var fechaBase = b.FechaReservaUtc ?? b.FechaCompra;
                var vencePorTtl = fechaBase.HasValue && (now - fechaBase.Value).TotalMinutes > ttlMin;

                // Ventana global (2h antes de salida)
                var vencePorVentana = false;
                if (viajes.TryGetValue(b.IdViaje, out var v))
                {
                    var salidaLocal = v.Fecha.Date + v.HoraSalida;   // DateOnly + TimeSpan => DateTime local
                    var limite = salidaLocal.AddHours(-ventanaHoras); // T–2h (por defecto)
                    vencePorVentana = now >= limite;
                }

                if (vencePorTtl || vencePorVentana)
                {
                    b.Estado = "ANULADO";
                    await _boletoRepo.UpdateAsync(b);

                    result.TotalExpiradas++;
                    if (vencePorTtl) result.ExpiradasPorTtl++;
                    if (vencePorVentana) result.ExpiradasPorVentana++;
                    result.ExpiradasIds.Add(b.IdBoleto);
                }
            }

            var msg = result.TotalExpiradas > 0
                ? $"Reservas expiradas: {result.TotalExpiradas} (TTL: {result.ExpiradasPorTtl}, Ventana: {result.ExpiradasPorVentana})."
                : "No había reservas para expirar.";

            return new Response<ExpirarReservasResultDto>(result, msg);
        }
    }
}
