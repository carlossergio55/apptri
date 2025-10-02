using Infraestructura.Abstract;
using Infraestructura.Models.Integracion;
using MudBlazor;
using Server.Shared.Component; // donde vive MainBaseComponent
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Pages.Pages.Admin_Integracion
{
    public partial class TarifaTramoAdmin : MainBaseComponent
    {
        // ---- Endpoints (mismos prefijos que usas en el resto del admin) ----
        private const string EP_RUTA_LIST = "Ruta/ruta";
        private const string EP_RUTA_PARADAS = "RutaParada/GetAllIdRutaParada"; // ?idRuta={id}
        private const string EP_TARIFA_LIST = "TarifaTramo/tarifa-tramo";      // ?idRuta={id}
        private const string EP_TARIFA_SAVE = "TarifaTramo/guardar";
        private const string EP_TARIFA_PUT = "TarifaTramo/tarifa-tramo";      // /{idTarifaTramo}
        private const string EP_TARIFA_DEL = "TarifaTramo/tarifa-tramo";      // /{idTarifaTramo}

        // ---- Estado UI ----
        protected int _idRuta = 1;
        protected bool _editando = false;

        protected List<RutaDto> _rutas = new();
        protected List<ParadaView> _paradas = new();
        protected List<TarifaTramoDto> _tarifas = new();

        protected TarifaTramoDto _form = new()
        {
            IdTarifaTramo = null,
            IdRuta = 1,
            OrigenParadaId = 0,
            DestinoParadaId = 0,
            Precio = 0.01M
        };

        // ---- Ciclo de vida ----
        protected override async Task OnInitializedAsync()
        {
            await CargarRutasAsync();
            await CargarParadasAsync();
            await CargarTarifasAsync();
        }

        // ---- Cargas ----
        private async Task CargarRutasAsync()
        {
            var r = await _Rest.GetAsync<List<RutaDto>>(EP_RUTA_LIST);
            if (r.State == State.Success && r.Data is not null)
            {
                _rutas = r.Data;
                if (_rutas.Count > 0 && !_rutas.Any(x => x.IdRuta == _idRuta))
                    _idRuta = _rutas[0].IdRuta;

                _form.IdRuta = _idRuta;
            }
            else
            {
                _MessageShow(r.Message ?? "No fue posible cargar rutas.", State.Error);
            }
        }

        private async Task CargarParadasAsync()
        {
            _paradas.Clear();
            var r = await _Rest.GetAsync<List<RutaParadaDto>>($"{EP_RUTA_PARADAS}?idRuta={_idRuta}");
            if (r.State == State.Success && r.Data is not null)
            {
                _paradas = r.Data
                    .OrderBy(p => p.Orden)
                    .Select(p => new ParadaView
                    {
                        IdParada = p.Parada?.IdParada ?? p.IdParada,
                        Nombre = p.Parada?.Nombre ?? "(parada)"
                    })
                    .ToList();
            }
            else
            {
                _MessageShow(r.Message ?? "No fue posible cargar paradas.", State.Error);
            }
        }

        private async Task CargarTarifasAsync()
        {
            var r = await _Rest.GetAsync<List<TarifaTramoDto>>($"{EP_TARIFA_LIST}?idRuta={_idRuta}");
            if (r.State == State.Success && r.Data is not null)
                _tarifas = r.Data;
            else
            {
                _tarifas = new();
                _MessageShow(r.Message ?? "No fue posible cargar tarifas.", State.Error);
            }
        }

        // ---- Eventos UI ----
        protected async Task OnRutaChanged(int nuevaRutaId)
        {
            _idRuta = nuevaRutaId;
            _form.IdRuta = _idRuta;
            LimpiarForm();
            await CargarParadasAsync();
            await CargarTarifasAsync();
            StateHasChanged();
        }

        protected void LimpiarForm()
        {
            _editando = false;
            _form = new TarifaTramoDto
            {
                IdTarifaTramo = null,
                IdRuta = _idRuta,
                OrigenParadaId = 0,
                DestinoParadaId = 0,
                Precio = 0.01M
            };
        }

        protected string NombreParada(int idParada) =>
            _paradas.FirstOrDefault(p => p.IdParada == idParada)?.Nombre ?? $"#{idParada}";

        protected void Editar(TarifaTramoDto t)
        {
            _editando = true;
            _form = new TarifaTramoDto
            {
                IdTarifaTramo = t.IdTarifaTramo,
                IdRuta = t.IdRuta,
                OrigenParadaId = t.OrigenParadaId,
                DestinoParadaId = t.DestinoParadaId,
                Precio = t.Precio
            };
        }

        protected async Task EliminarAsync(TarifaTramoDto t)
        {
            await _MessageConfirm(
                $"¿Eliminar la tarifa {NombreParada(t.OrigenParadaId)} → {NombreParada(t.DestinoParadaId)}?",
                async () =>
                {
                    if (!(t.IdTarifaTramo is int id) || id <= 0)
                    {
                        _MessageShow("Id de tarifa inválido.", State.Error);
                        return;
                    }

                    var r = await _Rest.DeleteAsync<int>(EP_TARIFA_DEL, id);
                    if (r.Succeeded)
                    {
                        _MessageShow("Tarifa eliminada.", State.Success);
                        await CargarTarifasAsync();
                        StateHasChanged();
                    }
                    else
                    {
                        _MessageShow(r.Message ?? "No se pudo eliminar.", State.Error);
                    }
                });
        }

        protected async Task GuardarAsync()
        {
            if (_form.IdRuta <= 0) { _MessageShow("Seleccione una ruta.", State.Warning); return; }
            if (_form.OrigenParadaId <= 0 || _form.DestinoParadaId <= 0) { _MessageShow("Seleccione origen y destino.", State.Warning); return; }
            if (_form.OrigenParadaId == _form.DestinoParadaId) { _MessageShow("El origen y destino no pueden ser iguales.", State.Warning); return; }
            if (_form.Precio <= 0) { _MessageShow("Ingrese un precio válido.", State.Warning); return; }

            try
            {
                _Loading.Show();

                if (_editando && _form.IdTarifaTramo is int id && id > 0)
                {
                    var r = await _Rest.PutAsync<int>(EP_TARIFA_PUT, _form, id);
                    if (r.State == State.Success)
                    {
                        _MessageShow("Tarifa actualizada.", State.Success);
                        await CargarTarifasAsync();
                        LimpiarForm();
                    }
                    else
                    {
                        _MessageShow(r.Message ?? "No se pudo actualizar.", State.Error);
                    }
                }
                else
                {
                    var r = await _Rest.PostAsync<int?>(EP_TARIFA_SAVE, new { Tarifa = _form });
                    if (r.State == State.Success && r.Data.GetValueOrDefault() > 0)
                    {
                        _MessageShow("Tarifa creada.", State.Success);
                        await CargarTarifasAsync();
                        LimpiarForm();
                    }
                    else
                    {
                        _MessageShow(r.Message ?? "No se pudo crear la tarifa.", State.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                _MessageShow($"Error al guardar: {ex.Message}", State.Error);
            }
            finally
            {
                _Loading.Hide();
            }
        }

        // ---- DTOs mínimos usados en esta pantalla ----
        public class RutaDto
        {
            public int IdRuta { get; set; }
            public string Origen { get; set; } = "";
            public string Destino { get; set; } = "";
        }

        public class RutaParadaDto
        {
            public int IdRutaParada { get; set; }
            public int IdRuta { get; set; }
            public int IdParada { get; set; }
            public int Orden { get; set; }
            public ParadaDto? Parada { get; set; }
        }

        public class ParadaDto
        {
            public int IdParada { get; set; }
            public string Nombre { get; set; } = "";
        }

        public class ParadaView
        {
            public int IdParada { get; set; }
            public string Nombre { get; set; } = "";
        }
    }
}
