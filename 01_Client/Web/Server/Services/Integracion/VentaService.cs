using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;                    
using Infraestructura.Models.Integracion;
using Web.Infraestructura.Models.Integracion;
using Server.Models;

namespace Server.Services.Integracion
{
    public class VentaService
    {
        private readonly HttpClient _http;

        public VentaService(HttpClient http)
        {
            _http = http;
        }

        // ===== Planilla de viajes =====
        public async Task<Response<List<ViajePlanillaDto>>> GetPlanilla(DateTime fecha, int dias)
        {
            var url = $"viaje/planilla?fecha={fecha:yyyy-MM-dd}&dias={dias}";
            return await _http.GetFromJsonAsync<Response<List<ViajePlanillaDto>>>(url);
        }

        public async Task<List<ParadaDto>> GetParadas(int idViaje)
        {
            var resp = await _http.GetFromJsonAsync<Response<List<ParadaDto>>>($"viaje/{idViaje}/paradas");
            return resp?.Data ?? new List<ParadaDto>();
        }

        public async Task<List<SeatmapSeatDto>> GetSeatmap(int viajeId, int origenId, int destinoId)
        {
            var url = $"viaje/{viajeId}/seatmap?origenId={origenId}&destinoId={destinoId}";
            var resp = await _http.GetFromJsonAsync<Response<List<SeatmapSeatDto>>>(url);
            return resp?.Data ?? new List<SeatmapSeatDto>();
        }

        public async Task Reservar(int viajeId, int asientoId, int origenId, int destinoId, ClienteDto cliente)
        {
            var url = $"venta/reservar";
            var payload = new
            {
                IdViaje = viajeId,
                IdAsiento = asientoId,
                OrigenParadaId = origenId,
                DestinoParadaId = destinoId,
                Cliente = cliente
            };
            await _http.PostAsJsonAsync(url, payload);
        }

        public async Task Confirmar(
            int viajeId,
            List<SeatmapSeatDto> asientos,
            ClienteDto cliente,
            decimal precio,
            string metodoPago,
            string referenciaPago)
        {
            var url = $"venta/confirmar";
            var payload = new
            {
                IdViaje = viajeId,
                Asientos = asientos.Select(a => a.IdAsiento).ToList(),
                Cliente = cliente,
                Precio = precio,
                MetodoPago = metodoPago,
                ReferenciaPago = referenciaPago
            };
            await _http.PostAsJsonAsync(url, payload);
        }
    }
}
