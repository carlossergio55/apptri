using Infraestructura.Models.Integracion;
using MudBlazor;
using Microsoft.AspNetCore.Components.Web; // KeyboardEventArgs
using System;
using System.Threading.Tasks;
using Infraestructura.Abstract;

namespace Server.Pages.Pages.Integracion
{
    public partial class RastrearEncomienda
    {
        private string numeroGuia = string.Empty;
        private EncomiendaPublicDto? EncomiendaEncontrada;
        private bool MostrandoResultado => EncomiendaEncontrada is not null;

        private async Task OnBuscarKeyDown(KeyboardEventArgs e)
        {
            if (e?.Key == "Enter")
                await BuscarPorGuia();
        }

        protected async Task BuscarPorGuia()
        {
            var guia = (numeroGuia ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(guia))
            {
                _MessageShow("Debe ingresar un número de guía", State.Warning);
                return;
            }

            try
            {
                _Loading.Show();

                // Usa el endpoint público: GET /Encomienda/track/{guiacarga}
                var url = $"Encomienda/track/{Uri.EscapeDataString(guia)}";
                var result = await _Rest.GetAsync<EncomiendaPublicDto>(url);

                if (result.State == State.Success && result.Data is not null)
                {
                    EncomiendaEncontrada = result.Data;
                }
                else
                {
                    EncomiendaEncontrada = null;
                    _MessageShow($"No se encontró la guía: {guia}", State.Error);
                }
            }
            catch (Exception ex)
            {
                EncomiendaEncontrada = null;
                _MessageShow($"Error al buscar la guía: {ex.Message}", State.Error);
            }
            finally
            {
                _Loading.Hide();
            }
        }

        // Helpers de presentación (sin más llamadas)
        private string RutaLabel(EncomiendaPublicDto e)
        {
            var origen = string.IsNullOrWhiteSpace(e.OrigenParadaNombre) ? "—" : e.OrigenParadaNombre;
            var destino = string.IsNullOrWhiteSpace(e.DestinoParadaNombre) ? "—" : e.DestinoParadaNombre;
            var label = $"{origen} → {destino}";
            return label == "— → —" ? $"Ruta {e.IdRuta}" : label;
        }

        private Color GetEstadoColor(string? estado)
        {
            if (string.IsNullOrWhiteSpace(estado)) return Color.Default;

            switch (estado.Trim().ToLowerInvariant())
            {
                case "en camino": return Color.Info;
                case "en destino": return Color.Warning;
                case "entregado": return Color.Success;
             
                default: return Color.Default;
            }
        }
        // Color del chip
     

        // Clase CSS para el contenedor del timeline
        private string GetEstadoCss(string? estado)
        {
            return (estado ?? "").Trim().ToLowerInvariant() switch
            {
                "en camino" => "encamino",
                "en destino" => "endestino",
                "entregado" => "entregado",
                
                _ => "desconocido"
            };
        }

        // Paso actual 1..4
        private int GetEstadoStep(string? estado)
        {
            return (estado ?? "").Trim().ToLowerInvariant() switch
            {
                "en camino" => 1,
                "en destino" => 2,
                "entregado" => 3,
                
                _ => 0
            };
        }

        // Clase para cada “bolita” del timeline
        private string StepClass(int step)
        {
            var current = GetEstadoStep(EncomiendaEncontrada?.Estado);
            if (step < current) return "tl-step done";
            if (step == current) return "tl-step active";
            return "tl-step";
        }

    }
}
