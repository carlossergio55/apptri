using Infraestructura.Abstract;
using Infraestructura.Models.Integracion;
using Microsoft.AspNetCore.Components.Forms;
using Server.Shared.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Pages.Pages.Admin_Integracion
{
    public partial class BusAdmin : MainBaseComponent
    {
        private bool expande = false;

        public BusDto _Bus = new();
        public List<BusDto> _buses = new();

        // --- Endpoints usados (ajusta si tu API usa otros nombres)
        private const string EP_BUS_LIST = "Bus/bus";
        private const string EP_BUS_SAVE = "Bus/guardar";
        private const string EP_BUS_PUT = "Bus";
        private const string EP_BUS_DEL = "Bus";

        private const string EP_ASIENTO_LIST = "Asiento/asiento";
        private const string EP_ASIENTO_SAVE = "Asiento/guardar";
        private const string EP_ASIENTO_DEL = "Asiento";

        private const string EP_BOLETO_LIST = "Boleto/boleto";

        // ---------- CRUD ----------
        private async Task GetBuses()
        {
            var res = await _Rest.GetAsync<List<BusDto>>(EP_BUS_LIST);
            if (res.State == State.Success && res.Data != null)
                _buses = res.Data;
            else
                _MessageShow(res.Message ?? "No se pudo obtener buses.", State.Warning);
        }

        private async Task Save(BusDto dto)
        {
            _Loading.Show();
            var r = await _Rest.PostAsync<int?>(EP_BUS_SAVE, new { Bus = dto });
            _Loading.Hide();

            if (r.State == State.Success && r.Data.HasValue && r.Data.Value > 0)
            {
                dto.IdBus = r.Data.Value;
                _MessageShow("Bus guardado.", State.Success);
                // Asegura plantilla de asientos 1..capacidad
                await EnsureSeats(dto);
            }
            else
            {
                _MessageShow(r.Message ?? "No se pudo guardar el bus.", State.Error);
                if (r.Errors != null) r.Errors.ForEach(e => _MessageShow(e, State.Warning));
            }
        }

        private async Task Update(BusDto dto)
        {
            _Loading.Show();
            var r = await _Rest.PutAsync<int>(EP_BUS_PUT, dto, dto.IdBus);
            _Loading.Hide();

            if (r.State == State.Success)
            {
                _MessageShow("Bus actualizado.", State.Success);
                // Asegura plantilla de asientos 1..capacidad
                await EnsureSeats(dto);
            }
            else
            {
                _MessageShow(r.Message ?? "No se pudo actualizar el bus.", State.Error);
            }
        }

        private async Task Eliminar(int id)
        {
            await _MessageConfirm("¿Eliminar el registro?", async () =>
            {
                var r = await _Rest.DeleteAsync<int>(EP_BUS_DEL, id);
                if (!r.Succeeded)
                {
                    _MessageShow(r.Message ?? "No se pudo eliminar.", State.Error);
                }
                else
                {
                    _MessageShow(r.Message, r.State);
                    await GetBuses();
                    StateHasChanged();
                }
            });
        }

        // ---------- Formulario ----------
        private async Task OnValidBus(EditContext _)
        {
            if (_Bus.IdBus > 0)
                await Update(_Bus);
            else
                await Save(_Bus);

            _Bus = new BusDto();
            await GetBuses();
            ToggleExpand();
        }

        private void FormEditar(BusDto dto)
        {
            _Bus = new BusDto
            {
                IdBus = dto.IdBus,
                Placa = dto.Placa,
                Modelo = dto.Modelo,
                Capacidad = dto.Capacidad
            };
            ToggleExpand();
        }

        private void ResetBus() => _Bus = new BusDto();
        private void ToggleExpand() => expande = !expande;

        // ---------- Ensure seats (1..capacidad) ----------
        private async Task EnsureSeats(BusDto bus)
        {
            try
            {
                _Loading.Show();

                // 1) Asientos actuales del bus
                var rA = await _Rest.GetAsync<List<AsientoDto>>(EP_ASIENTO_LIST);
                var actuales = (rA.State == State.Success && rA.Data != null)
                    ? rA.Data.Where(a => a.IdBus == bus.IdBus).ToList()
                    : new List<AsientoDto>();

                var actualesPorNumero = actuales.ToDictionary(a => a.Numero, a => a);

                // 2) Crear faltantes en 1..capacidad
                var faltan = new List<int>();
                for (int n = 1; n <= bus.Capacidad; n++)
                {
                    if (!actualesPorNumero.ContainsKey(n))
                        faltan.Add(n);
                }

                foreach (var num in faltan)
                {
                    var nuevo = new AsientoDto { IdAsiento = 0, IdBus = bus.IdBus, Numero = num };
                    var rSave = await _Rest.PostAsync<int?>(EP_ASIENTO_SAVE, new { Asiento = nuevo });
                    if (rSave.State != State.Success || !rSave.Data.HasValue || rSave.Data.Value <= 0)
                        _MessageShow($"No se pudo crear asiento {num} del bus {bus.Placa}.", State.Warning);
                }

                // 3) Intentar eliminar sobrantes (número > capacidad), si no tienen boletos
                var sobrantes = actuales.Where(a => a.Numero > bus.Capacidad).OrderBy(a => a.Numero).ToList();
                var conflictos = new List<int>();

                if (sobrantes.Count > 0)
                {
                    // Obtener boletos para detectar uso
                    var rB = await _Rest.GetAsync<List<BoletoDto>>(EP_BOLETO_LIST);
                    var boletos = (rB.State == State.Success && rB.Data != null) ? rB.Data : new List<BoletoDto>();

                    foreach (var s in sobrantes)
                    {
                        var usado = boletos.Any(b => b.IdAsiento == s.IdAsiento);
                        if (usado)
                        {
                            conflictos.Add(s.Numero);
                            continue;
                        }

                        var rDel = await _Rest.DeleteAsync<int>(EP_ASIENTO_DEL, s.IdAsiento);
                        if (!rDel.Succeeded)
                            _MessageShow($"No se pudo eliminar asiento {s.Numero}: {rDel.Message}", State.Warning);
                    }
                }

                // 4) Mensajes finales
                if (faltan.Count == 0 && sobrantes.Count == 0)
                    _MessageShow("Plantilla de asientos: ya estaba correcta.", State.Success);
                else
                    _MessageShow($"Plantilla sincronizada. Creados: {faltan.Count}. Intento eliminar sobrantes: {sobrantes.Count}.", State.Success);

                if (conflictos.Count > 0)
                    _MessageShow($"No se pudieron eliminar asientos {string.Join(", ", conflictos)} porque tienen boletos asociados.", State.Warning);
            }
            catch (Exception ex)
            {
                _MessageShow($"Error sincronizando asientos: {ex.Message}", State.Error);
            }
            finally
            {
                _Loading.Hide();
            }
        }

        // ---------- Init ----------
        protected override async Task OnInitializedAsync()
        {
            await GetBuses();
        }
    }

    // ---- DTOs usados por esta página (ya existen en tu proyecto, aquí solo para referencia) ----
    public class BusDto
    {
        public int IdBus { get; set; }
        public string Placa { get; set; }
        public string Modelo { get; set; }
        public int Capacidad { get; set; }
    }

    public class AsientoDto
    {
        public int IdAsiento { get; set; }
        public int IdBus { get; set; }
        public int Numero { get; set; }
    }

    public class BoletoDto
    {
        public int IdBoleto { get; set; }
        public int IdViaje { get; set; }
        public int IdCliente { get; set; }
        public int IdAsiento { get; set; }
        public decimal Precio { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCompra { get; set; }
        public int? OrigenParadaId { get; set; }
        public int? DestinoParadaId { get; set; }
    }
}
