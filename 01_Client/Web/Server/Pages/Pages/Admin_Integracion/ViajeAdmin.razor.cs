using Infraestructura.Abstract;
using Infraestructura.Models.Integracion;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Server.Pages.Pages.Admin_Integracion
{
    public partial class ViajeAdmin : IDisposable
    {
        private bool expande = false;

        public ViajeDto _Viaje = new();

        public List<ViajeDto> _viajes = new();
        public List<ChoferDto> _chofer = new();
        public List<BusDto> _bus = new();
        public List<RutaDto> _rutas = new();

        // Horas predefinidas para el selector
        private readonly string[] _horasPreset = { "09:00", "12:00", "13:30", "14:00", "17:30" };

        // Mask para HH:mm
        private readonly PatternMask _maskHora = new("00:00");

        // backing fields
        private DateTime? _fechaSel;
        private int? _choferSel;
        private int? _busSel;

        // ====== Auto refresh / control de reentradas ======
        private Timer? _timer;
        private bool _isDisposed;
        private bool _isRefreshing;
        private DateTime _desdeEstado = DateTime.Today.AddDays(-1);
        private DateTime _hastaEstado = DateTime.Today.AddDays(2);

        // Control de finalizados
        private bool mostrarFinalizados = false;

        // ---------- Helpers UI ----------
        protected string NombreRuta(int idRuta)
        {
            var r = _rutas.FirstOrDefault(x => x.IdRuta == idRuta);
            return r is null ? $"Ruta {idRuta}" : $"{r.Origen} → {r.Destino}";
        }

        protected string NombreChofer(int id) => _chofer.FirstOrDefault(c => c.IdChofer == id)?.Nombre ?? "-";
        protected string NombreBus(int id) => _bus.FirstOrDefault(b => b.IdBus == id)?.Placa ?? "-";

        protected Color EstadoColor(string? estado)
        {
            var e = (estado ?? "PROGRAMADO").Trim().ToUpperInvariant();
            return e switch
            {
                "PROGRAMADO" => Color.Info,
                "EMBARCANDO" => Color.Warning,
                "ENRUTA" => Color.Primary,
                "FINALIZADO" => Color.Success,
                "CANCELADO" => Color.Error,
                _ => Color.Default
            };
        }

        // Detecta viajes extendidos (Jue/Dom 17:30). Acepta "HH:mm" o "HH:mm:ss"
        private static bool EsExtendido(ViajeDto v)
        {
            var s = (v.HoraSalida ?? "").Trim();
            if (string.IsNullOrEmpty(s)) return false;
            if (s.Length >= 5) s = s[..5]; // normaliza "17:30:00" -> "17:30"
            return s == "17:30" &&
                   (v.Fecha.DayOfWeek == DayOfWeek.Thursday || v.Fecha.DayOfWeek == DayOfWeek.Sunday);
        }

        // ---------- API helpers ----------
        private async Task ActualizarEstados(bool mostrarMensaje = false)
        {
            var body = new { Desde = _desdeEstado, Hasta = _hastaEstado };
            var r = await _Rest.PostAsync<object>("Viaje/actualizar-estados", body);
            if (mostrarMensaje) _MessageShow(r.Message ?? "Estados actualizados.", r.State);
        }

        /// <summary>
        /// Refresca estados + lista, evitando reentradas del timer y errores de render.
        /// </summary>
        private async Task ActualizarYRefrescarAsync(bool showToast = false)
        {
            if (_isDisposed || _isRefreshing) return;
            _isRefreshing = true;
            try
            {
                await ActualizarEstados(showToast);

                var res = await _Rest.GetAsync<List<ViajeDto>>("Viaje/viaje");
                if (res.State == State.Success) _viajes = res.Data ?? new();
                else _MessageShow(res.Message, State.Warning);

                if (!_isDisposed) StateHasChanged();
            }
            catch (Exception ex)
            {
                _MessageShow($"Error al refrescar viajes: {ex.Message}", State.Error);
            }
            finally
            {
                _isRefreshing = false;
            }
        }

        // ---------- CRUD ----------
        private async Task GetViajes() => await ActualizarYRefrescarAsync();

        private async Task GetChofer()
        {
            var res = await _Rest.GetAsync<List<ChoferDto>>("Chofer/chofer");
            if (res.State == State.Success) _chofer = res.Data ?? new();
            else _MessageShow(res.Message, State.Warning);
        }

        private async Task GetBus()
        {
            var res = await _Rest.GetAsync<List<BusDto>>("Bus/bus");
            if (res.State == State.Success) _bus = res.Data ?? new();
            else _MessageShow(res.Message, State.Warning);
        }

        private async Task GetRutas()
        {
            var res = await _Rest.GetAsync<List<RutaDto>>("Ruta/ruta");
            if (res.State == State.Success) _rutas = res.Data ?? new();
            else _MessageShow(res.Message, State.Warning);
        }

        private async Task Save(ViajeDto dto)
        {
            try
            {
                _Loading.Show();
                var r = await _Rest.PostAsync<int?>("Viaje/guardar", new { Viaje = dto });
                _MessageShow(r.Message, r.State);
                if (r.State != State.Success && r.Errors != null)
                    foreach (var e in r.Errors) _MessageShow(e, State.Warning);
            }
            finally
            {
                _Loading.Hide();
            }
        }

        private async Task Update(ViajeDto dto)
        {
            try
            {
                _Loading.Show();
                var r = await _Rest.PutAsync<int>("Viaje", dto, dto.IdViaje);
                _MessageShow(r.Message, r.State);
            }
            finally
            {
                _Loading.Hide();
            }
        }

        private async Task Eliminar(int id)
        {
            await _MessageConfirm("¿Eliminar el registro?", async () =>
            {
                var r = await _Rest.DeleteAsync<int>("Viaje", id);
                if (!r.Succeeded)
                    _MessageShow(r.Message, State.Error);
                else
                {
                    _MessageShow(r.Message, r.State);
                    await GetViajes();
                    StateHasChanged();
                }
            });
        }

        // ---------- Form ----------
        private async Task OnValidViaje(EditContext _)
        {
            // Sincronizar backing fields -> DTO
            _Viaje.Fecha = _fechaSel ?? DateTime.Today;
            if (_choferSel.HasValue) _Viaje.IdChofer = _choferSel.Value;
            if (_busSel.HasValue) _Viaje.IdBus = _busSel.Value;

            if (_Viaje.IdViaje > 0) await Update(_Viaje);
            else await Save(_Viaje);

            ResetViaje();
            await GetViajes();
            Collapse();
        }

        private void FormEditar(ViajeDto dto)
        {
            _Viaje = new ViajeDto
            {
                IdViaje = dto.IdViaje,
                Fecha = dto.Fecha,
                HoraSalida = dto.HoraSalida,
                Estado = dto.Estado,
                Direccion = dto.Direccion,
                DesdeParadaId = dto.DesdeParadaId,
                HastaParadaId = dto.HastaParadaId,
                IdRuta = dto.IdRuta,
                IdChofer = dto.IdChofer,
                IdBus = dto.IdBus
            };

            _fechaSel = dto.Fecha;
            _choferSel = dto.IdChofer;
            _busSel = dto.IdBus;

            Expand();
        }

        private void ResetViaje()
        {
            _Viaje = new ViajeDto
            {
                HoraSalida = "09:00",
                Estado = "PROGRAMADO",
                Direccion = "IDA"
            };
            _fechaSel = DateTime.Today;
            _choferSel = null;
            _busSel = null;
        }

        private void Expand() => expande = true;
        private void Collapse() => expande = false;

        // ---------- Generar automáticos ----------
        protected async Task GenerarProximos(int dias)
        {
            try
            {
                _Loading.Show();
                var body = new { Desde = DateTime.Now.Date, Dias = dias };
                var r = await _Rest.PostAsync<object>("Viaje/generar-proximos", body);
                _MessageShow(r.Message ?? "Generación completada.", r.State);

                await ActualizarYRefrescarAsync(showToast: true);
            }
            catch (Exception ex)
            {
                _MessageShow($"Error al generar viajes: {ex.Message}", State.Error);
            }
            finally
            {
                _Loading.Hide();
            }
        }

        // ---------- Init / Dispose ----------
        protected override async Task OnInitializedAsync()
        {
            ResetViaje();
            await GetRutas();
            await GetChofer();
            await GetBus();
            await GetViajes();

            // refrescar automáticamente cada 60s (estados + lista) evitando reentradas
            _timer = new Timer(60_000)
            {
                AutoReset = true,
                Enabled = true
            };
            _timer.Elapsed += async (_, __) =>
            {
                if (_isDisposed) return;
                try
                {
                    await InvokeAsync(async () => await ActualizarYRefrescarAsync());
                }
                catch { /* swallow to avoid breaking render loop */ }
            };
            _timer.Start();
        }

        public void Dispose()
        {
            _isDisposed = true;
            if (_timer != null)
            {
                try { _timer.Stop(); }
                catch { /* noop */ }
                _timer.Dispose();
                _timer = null;
            }
        }

        // ---------- Vista organizada ----------
        private IEnumerable<ViajeDto> ViajesOrdenados =>
            _viajes
                .OrderBy(v => v.Fecha)
                .ThenBy(v => v.HoraSalida)
                .Where(v => mostrarFinalizados ||
                            !string.Equals(v.Estado, "FINALIZADO", StringComparison.OrdinalIgnoreCase));

        private void ToggleFinalizados() => mostrarFinalizados = !mostrarFinalizados;
    }
}
