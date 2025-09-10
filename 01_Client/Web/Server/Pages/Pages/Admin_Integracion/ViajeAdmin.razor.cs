using Infraestructura.Abstract;
using Infraestructura.Models.Integracion;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Pages.Pages.Admin_Integracion
{
    public partial class ViajeAdmin
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
            var e = (estado ?? "PROGRAMADO").ToUpperInvariant();
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

        // ---------- CRUD ----------
        private async Task GetViajes()
        {
            var res = await _Rest.GetAsync<List<ViajeDto>>("Viaje/viaje");
            if (res.State == State.Success) _viajes = res.Data ?? new();
            else _MessageShow(res.Message, State.Warning);
        }

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
            ToggleExpand();
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

            ToggleExpand();
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

        private void ToggleExpand() => expande = !expande;

        // ---------- Generar automáticos ----------
        protected async Task GenerarProximos(int dias)
        {
            try
            {
                _Loading.Show();
                var body = new { Desde = DateTime.Now.Date, Dias = dias };
                var r = await _Rest.PostAsync<object>("Viaje/generar-proximos", body);
                _MessageShow(r.Message ?? "Generación completada.", r.State);
                await GetViajes();
                StateHasChanged();
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

        // ---------- Init ----------
        protected override async Task OnInitializedAsync()
        {
            ResetViaje();
            await GetRutas();
            await GetChofer();
            await GetBus();
            await GetViajes();
        }
    }
}
