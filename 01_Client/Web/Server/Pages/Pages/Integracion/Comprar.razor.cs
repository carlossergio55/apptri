using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infraestructura.Abstract;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Linq;
using Infraestructura.Models.Clasificador;
using MudBlazor;
using Infraestructura.Models.Integracion;
using Microsoft.AspNetCore.Components;           // [Inject]
using Server;                                   // CompraFlowState

namespace Server.Pages.Pages.Integracion
{
    public partial class Comprar
    {
        [Inject] public CompraFlowState Flow { get; set; } = default!;

        private int _rutaSeleccionadaId;
        public List<RutaDto> _listaRutas = new();
        public List<string> _destinosDisponibles = new();   // ← paradas destino reales (Tomina, Villa Serrano, Mendoza, ...)

        // --- Campos que antes estaban en el .razor (déjalos SOLO aquí) ---
        public string OrigenSeleccionado { get; set; } = "Sucre";     // origen fijo
        public string DestinoSeleccionado { get; set; }
        public DateTime? FechaSalida { get; set; } = DateTime.Today;
        public int CantidadPasajeros { get; set; } = 1;

        // Ventana de compra (hoy .. +14)
        private DateTime Hoy => DateTime.Today;
        private DateTime MaxCompra => DateTime.Today.AddDays(14);

        // Regla específica: Mendoza solo Jueves y Domingo (informativa)
        private static readonly DayOfWeek[] DiasMendoza = new[] { DayOfWeek.Thursday, DayOfWeek.Sunday };

        #region DTOs mapeando tu JSON de /Viaje/viaje
        private class ViajeApiDto
        {
            public int IdViaje { get; set; }
            public DateTime Fecha { get; set; }
            public string HoraSalida { get; set; }
            public string Estado { get; set; }
            public string Direccion { get; set; }
            public int IdRuta { get; set; }
            public int IdChofer { get; set; }
            public int IdBus { get; set; }
        }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            Flow.Reset();
            await GetRutas();
            await CargarDestinosParaOrigenFijo(); // llena _destinosDisponibles desde RutaParada (para el combo)
        }

        protected async Task GetRutas()
        {
            try
            {
                _Loading.Show();
                var result = await _Rest.GetAsync<List<RutaDto>>("Ruta/ruta");
                _Loading.Hide();

                if (result.State == State.Success && result.Data != null)
                    _listaRutas = result.Data;
                else
                    _MessageShow($"Error: {result.Message}", State.Error);
            }
            catch (Exception ex)
            {
                _Loading.Hide();
                _MessageShow($"Excepción: {ex.Message}", State.Error);
            }
        }

        private async Task CargarDestinosParaOrigenFijo()
        {
            // Toma la primera ruta que parte de Sucre (en tu caso, “solo una ruta”)
            var ruta = _listaRutas.FirstOrDefault(r =>
                string.Equals(r.Origen, "Sucre", StringComparison.OrdinalIgnoreCase));

            if (ruta == null) return;

            // ⚠️ Ajusta si tu RutaDto usa 'Id' en vez de 'IdRuta'
            var idRuta = ruta.IdRuta;

            var resp = await _Rest.GetAsync<List<RutaParadaDto>>($"RutaParada/GetAllIdRutaParada?idRuta={idRuta}");
            if (resp.State != State.Success || resp.Data == null) return;

            // destinos válidos = todas las paradas con orden > 0 (excluye origen)
            _destinosDisponibles = resp.Data
                .Where(p => p.Orden > 0)
                .Select(p => p.Parada?.Nombre ?? string.Empty)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        // Handler sincrónico para el botón (el .razor lo invoca)
        protected void BuscarPasajes() => _ = BuscarPasajesAsync();

        protected async Task BuscarPasajesAsync()
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(DestinoSeleccionado))
            { _MessageShow("Seleccione ciudad de destino.", State.Warning); return; }

            if (FechaSalida is null || FechaSalida.Value.Date < Hoy || FechaSalida.Value.Date > MaxCompra)
            {
                _MessageShow($"Solo se puede comprar hasta {MaxCompra:dd/MM/yyyy} (máx. 2 semanas).", State.Warning);
                return;
            }

            // Regla informativa: Mendoza solo Jue/Dom
            if (DestinoSeleccionado.Equals("Mendoza", StringComparison.OrdinalIgnoreCase) &&
                !(FechaSalida.Value.DayOfWeek is DayOfWeek.Thursday or DayOfWeek.Sunday))
            {
                var suger = ProximasFechasJueDom(FechaSalida.Value.Date, MaxCompra)
                            .Take(4)
                            .Select(d => d.ToString("dd/MM"));
                var msg = "A Mendoza solo hay salidas los jueves y domingo.";
                var extras = string.Join(", ", suger);
                if (!string.IsNullOrEmpty(extras)) msg += $" Próximas fechas: {extras}.";
                _MessageShow(msg, State.Success);
                // seguimos buscando por si existen excepciones cargadas
            }

            try
            {
                // 1) Determinar la ruta (Sucre -> …)
                var ruta = _listaRutas.FirstOrDefault(r =>
                    string.Equals(r.Origen, "Sucre", StringComparison.OrdinalIgnoreCase));
                if (ruta == null)
                { _MessageShow("No se encontró una ruta que parta de Sucre.", State.Success); return; }

                // ⚠️ Ajusta si tu RutaDto usa 'Id' en vez de 'IdRuta'
                var idRuta = ruta.IdRuta;

                // 2) Validar que el destino elegido exista en las paradas de esa ruta
                var respParadas = await _Rest.GetAsync<List<RutaParadaDto>>($"RutaParada/GetAllIdRutaParada?idRuta={idRuta}");
                if (respParadas.State != State.Success || respParadas.Data == null)
                { _MessageShow("No fue posible obtener las paradas de la ruta.", State.Error); return; }

                var destinosRuta = respParadas.Data
                    .Where(p => p.Orden > 0)
                    .Select(p => p.Parada?.Nombre ?? string.Empty)
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                if (!destinosRuta.Contains(DestinoSeleccionado))
                {
                    _MessageShow($"El destino {DestinoSeleccionado} no pertenece a la ruta seleccionada.", State.Success);
                    return;
                }

                // 3) Obtener viajes crudos
                var respViajes = await _Rest.GetAsync<List<ViajeApiDto>>("Viaje/viaje");
                if (respViajes.State != State.Success || respViajes.Data == null)
                { _MessageShow($"No fue posible consultar los viajes: {respViajes.Message}", State.Error); return; }

                // 4) Filtrar por esa ruta, estado y ventana de compra
                var fecha = FechaSalida.Value.Date;

                var viajesDestino = respViajes.Data
                    .Where(v => v.Estado == "PROGRAMADO"
                             && v.IdRuta == idRuta
                             && v.Fecha.Date >= Hoy && v.Fecha.Date <= MaxCompra)
                    .ToList();

                if (viajesDestino.Count == 0)
                {
                    _MessageShow($"No hay viajes disponibles a {DestinoSeleccionado} en las próximas 2 semanas.", State.Success);
                    return;
                }

                // 5) Viajes del día elegido
                var delDia = viajesDestino
                    .Where(v => v.Fecha.Date == fecha)
                    .OrderBy(v => ParseHora(v.HoraSalida))
                    .ToList();

                if (delDia.Count == 0)
                {
                    var proximas = viajesDestino.Select(v => v.Fecha.Date)
                                                .Distinct()
                                                .OrderBy(d => d)
                                                .Take(4)
                                                .Select(d => d.ToString("dd/MM"));
                    _MessageShow($"No hay viajes disponibles para esa fecha. Próximas fechas: {string.Join(", ", proximas)}.", State.Success);
                    return;
                }

                // 6) Ir a Asientos con el primer viaje del día (o muestra selección si prefieres)
                var seleccion = delDia.First();
                Nav.NavigateTo($"/comprar-asiento?viajeId={seleccion.IdViaje}&pax={CantidadPasajeros}");
            }
            catch (Exception ex)
            {
                _MessageShow($"Error al buscar viajes: {ex.Message}", State.Error);
            }
        }
        public void AbrirTutorial()
        {
            // Reemplaza VIDEO_ID cuando tengas el definitivo
            Nav.NavigateTo("https://www.youtube.com/watch?v=VIDEO_ID", true);
        }
        public void LimpiarCampos()
        {
            OrigenSeleccionado = "Sucre";     // si usas origen fijo
            DestinoSeleccionado = null;
            FechaSalida = DateTime.Today;
            CantidadPasajeros = 1;
        }
        private static TimeSpan ParseHora(string hhmmss)
            => TimeSpan.TryParse(hhmmss, out var ts) ? ts : TimeSpan.Zero;

        private static IEnumerable<DateTime> ProximasFechasJueDom(DateTime desde, DateTime hasta)
        {
            var d = desde;
            while (d <= hasta)
            {
                if (d.DayOfWeek is DayOfWeek.Thursday or DayOfWeek.Sunday)
                    yield return d;
                d = d.AddDays(1);
            }
        }
    }
}
