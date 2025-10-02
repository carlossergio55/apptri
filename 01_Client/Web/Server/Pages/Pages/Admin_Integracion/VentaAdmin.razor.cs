using Infraestructura.Models.Integracion;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Server.Services; // VentaService
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Services.Integracion;
using Web.Infraestructura.Models.Integracion;


namespace Server.Pages.Pages.Admin_Integracion
{
    public partial class VentaAdmin
    {
        [Inject] private VentaService VentaSvc { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }

        // Estado general
        private List<ViajePlanillaDto> _planilla = new();
        private bool IsLoading { get; set; }
        private DateTime? SelectedDate { get; set; } = DateTime.Today;
        private int _diasFiltro = 1;

        // Estado por viaje expandido
        private readonly Dictionary<int, RowUiState> _ui = new();

        // ====== Inicialización ======
        protected override async Task OnInitializedAsync()
        {
            await CargarPlanilla();
        }

        // ====== Cargar viajes ======
        protected async Task CargarPlanilla()
        {
            try
            {
                IsLoading = true;
                var resp = await VentaSvc.GetPlanilla(SelectedDate ?? DateTime.Today, _diasFiltro);

                _planilla = resp.Data
                    .OrderBy(v => v.Fecha)
                    .ThenBy(v => v.HoraSalida)
                    .ToList();

            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error cargando planilla: {ex.Message}", Severity.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ====== Filtros rápidos ======
        protected async Task SetHoy()
        {
            SelectedDate = DateTime.Today;
            await CargarPlanilla();
        }

        protected async Task SetManana()
        {
            SelectedDate = DateTime.Today.AddDays(1);
            await CargarPlanilla();
        }

        // ====== Expandir viaje ======
        protected async Task ToggleExpand(ViajePlanillaDto viaje)
        {
            if (_ui.ContainsKey(viaje.IdViaje))
            {
                _ui.Remove(viaje.IdViaje);
                return;
            }

            var state = new RowUiState
            {
                Viaje = viaje,
                OrigenParadaId = viaje.OrigenParadaId,
                DestinoParadaId = viaje.DestinoParadaId,
                Cliente = new ClienteDto()
            };

            state.Loading = true;
            _ui[viaje.IdViaje] = state;

            await CargarParadas(state);
            await CargarSeatmap(state);
        }

        // ====== Cargar paradas ======
        protected async Task CargarParadas(RowUiState u)
        {
            try
            {
                var paradas = await VentaSvc.GetParadas(u.Viaje.IdViaje);
                u.Paradas = paradas ?? new List<ParadaDto>();
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error cargando paradas: {ex.Message}", Severity.Error);
            }
        }

        // ====== Seatmap ======
        protected async Task CargarSeatmap(RowUiState u)
        {
            try
            {
                u.Loading = true;
                if (u.OrigenParadaId == 0 || u.DestinoParadaId == 0) return;

                u.Seatmap = await VentaSvc.GetSeatmap(u.Viaje.IdViaje, u.OrigenParadaId, u.DestinoParadaId);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error cargando seatmap: {ex.Message}", Severity.Error);
            }
            finally
            {
                u.Loading = false;
                StateHasChanged();
            }
        }

        // ====== Selección de asientos ======
        protected async Task OnSeatClick(RowUiState u, SeatmapSeatDto s)
        {
            if (s.EstadoSeat == "LIBRE")
            {
                if (!u.Pendientes.Contains(s.IdAsiento))
                    u.Pendientes.Add(s.IdAsiento);
            }
            else
            {
                Snackbar.Add($"Asiento {s.Numero} no disponible.", Severity.Warning);
            }
            await Task.CompletedTask;
        }

        protected async Task QuitarSeleccion(RowUiState u, SeatmapSeatDto s)
        {
            u.Pendientes.Remove(s.IdAsiento);
            u.Seleccionados.Remove(s.IdAsiento);
            await Task.CompletedTask;
        }

        // ====== Acciones ======
        protected async Task ReservarSeleccion(RowUiState u)
        {
            try
            {
                foreach (var idAsiento in u.Pendientes.ToList())
                {
                    await VentaSvc.Reservar(
                        u.Viaje.IdViaje,
                        idAsiento,
                        u.OrigenParadaId,
                        u.DestinoParadaId,
                        u.Cliente);

                    u.Seleccionados[idAsiento] = u.Seatmap.First(s => s.IdAsiento == idAsiento);
                    u.Pendientes.Remove(idAsiento);
                }

                Snackbar.Add("Reservas realizadas.", Severity.Success);
                await CargarSeatmap(u);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error al reservar: {ex.Message}", Severity.Error);
            }
        }

        protected async Task ConfirmarSeleccion(RowUiState u)
        {
            try
            {
                if (u.Seleccionados.Count == 0)
                {
                    Snackbar.Add("No hay asientos seleccionados para confirmar.", Severity.Warning);
                    return;
                }

                await VentaSvc.Confirmar(
                    u.Viaje.IdViaje,
                    u.Seleccionados.Values.ToList(),
                    u.Cliente,
                    u.PrecioUnitario,
                    u.MetodoPago,
                    u.ReferenciaPago);

                Snackbar.Add("Boletos confirmados y emitidos.", Severity.Success);
                u.Seleccionados.Clear();
                await CargarSeatmap(u);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error al confirmar: {ex.Message}", Severity.Error);
            }
        }

        protected async Task CancelarSeleccion(RowUiState u)
        {
            u.Pendientes.Clear();
            u.Seleccionados.Clear();
            Snackbar.Add("Selección cancelada.", Severity.Info);
            await Task.CompletedTask;
        }

        // ====== Helpers ======
        protected bool PuedeVender(string estado)
            => estado == "PROGRAMADO" || estado == "EMBARCANDO";

        protected Color EstadoColor(string estado) =>
            estado switch
            {
                "PROGRAMADO" => Color.Info,
                "EMBARCANDO" => Color.Warning,
                "CERRADO" => Color.Dark,
                _ => Color.Default
            };

        protected bool PermiteReservar(ViajePlanillaDto v)
        {
            var salida = v.Fecha.Date + TimeSpan.Parse(v.HoraSalida);
            return DateTime.Now < salida.AddHours(-2); // se desactiva <2h
        }

        protected bool PuedeConfirmar(RowUiState u)
            => u != null && u.Seleccionados.Count > 0 && u.PrecioUnitario > 0;

        // 🔹 Agrupar asientos por filas
        protected IEnumerable<List<SeatmapSeatDto>> GetSeatRows(List<SeatmapSeatDto> seats, int seatsPerRow = 4)
        {
            for (int i = 0; i < seats.Count; i += seatsPerRow)
                yield return seats.Skip(i).Take(seatsPerRow).ToList();
        }
    }
}
