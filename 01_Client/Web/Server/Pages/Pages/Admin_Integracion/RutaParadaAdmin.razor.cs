using Infraestructura.Abstract;
using Infraestructura.Models.Integracion;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Pages.Pages.Admin_Integracion
{
    public partial class RutaParadaAdmin
    {
        private bool expande = false;
        private bool _isSubmitting = false;

        public RutaParadaDto _RutaParada = new();
        public List<RutaParadaDto> _rutaParadas = new();
        public List<RutaDto> _rutas = new();
        public List<ParadaDto> _paradas = new();

        // Ruta seleccionada para filtrar
        private int? _rutaSel;

        // ---------- Helpers UI ----------
        private string NombreRuta(int idRuta)
        {
            var r = _rutas.FirstOrDefault(x => x.IdRuta == idRuta);
            return r is null ? $"Ruta {idRuta}" : $"{r.Origen} ? {r.Destino}";
        }

        private string NombreParada(int idParada)
        {
            var p = _paradas.FirstOrDefault(x => x.IdParada == idParada);
            return p is null ? $"Parada {idParada}" : p.Nombre;
        }

        // ---------- CRUD ----------
        private async Task GetRutaParadas()
        {
            try
            {
                var _result = await _Rest.GetAsync<List<RutaParadaDto>>("RutaParada/GetAllRutaParadaFull");
                _Loading.Hide();
                //_MessageShow(_result.Message, _result.State);
                if (_result.State != State.Success)
                    return;
                _rutaParadas = _result.Data;
            }
            catch (Exception e)
            {
                _MessageShow(e.Message, State.Error);
            }
        }

        private async Task GetRutas()
        {
            try
            {
                var res = await _Rest.GetAsync<List<RutaDto>>("Ruta/ruta");
                if (res.State == State.Success)
                    _rutas = res.Data ?? new();
                else
                    _MessageShow(res.Message, State.Warning);
            }
            catch (Exception ex)
            {
                _MessageShow($"Error al obtener rutas: {ex.Message}", State.Error);
            }
        }

        private async Task GetParadas()
        {
            try
            {
                var res = await _Rest.GetAsync<List<ParadaDto>>("Parada/parada");
                if (res.State == State.Success)
                    _paradas = res.Data ?? new();
                else
                    _MessageShow(res.Message, State.Warning);
            }
            catch (Exception ex)
            {
                _MessageShow($"Error al obtener paradas: {ex.Message}", State.Error);
            }
        }

        private async Task Save(RutaParadaDto dto)
        {
            try
            {
                _Loading.Show();
                var r = await _Rest.PostAsync<int?>("RutaParada/guardar", new { RutaParada = dto });
                _MessageShow(r.Message, r.State);

                if (r.State != State.Success && r.Errors != null)
                    foreach (var e in r.Errors)
                        _MessageShow(e, State.Warning);
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

        private async Task Update(RutaParadaDto dto)
        {
            try
            {
                _Loading.Show();
                var r = await _Rest.PutAsync<int>("RutaParada", dto, dto.IdRutaParada);
                _MessageShow(r.Message, r.State);
            }
            catch (Exception ex)
            {
                _MessageShow($"Error al actualizar: {ex.Message}", State.Error);
            }
            finally
            {
                _Loading.Hide();
            }
        }

        private async Task Eliminar(int idRutaParada)
        {
            await _MessageConfirm("¿Eliminar la asociación Ruta–Parada?", async () =>
            {
                try
                {
                    var r = await _Rest.DeleteAsync<int>("RutaParada", idRutaParada);
                    if (!r.Succeeded)
                        _MessageShow(r.Message, State.Error);
                    else
                    {
                        _MessageShow(r.Message, r.State);
                        await GetRutaParadas();
                        StateHasChanged();
                    }
                }
                catch (Exception ex)
                {
                    _MessageShow($"Error al eliminar: {ex.Message}", State.Error);
                }
            });
        }

        // ---------- Form ----------
        private async Task OnValidRutaParada(EditContext _)
        {
            if (_isSubmitting) return;
            _isSubmitting = true;

            try
            {
                if (_rutaSel.HasValue)
                    _RutaParada.IdRuta = _rutaSel.Value;

                if (_RutaParada.IdRutaParada > 0)
                    await Update(_RutaParada);
                else
                    await Save(_RutaParada);

                _RutaParada = new RutaParadaDto();
                await GetRutaParadas();
                ToggleExpand();
            }
            finally
            {
                _isSubmitting = false;
            }
        }

        private void FormEditar(RutaParadaDto dto)
        {
            _RutaParada = new RutaParadaDto
            {
                IdRutaParada = dto.IdRutaParada,
                IdRuta = dto.IdRuta,
                IdParada = dto.IdParada,
                Orden = dto.Orden
            };
            _rutaSel = dto.IdRuta;
            ToggleExpand();
        }

        private void OnRutaChanged(int? id)
        {
            _rutaSel = id;
            _ = GetRutaParadas();
        }

        private void ResetRutaParada() => _RutaParada = new RutaParadaDto();
        private void ToggleExpand() => expande = !expande;

        // ---------- Init ----------
        protected override async Task OnInitializedAsync()
        {
            await GetRutas();
            await GetParadas();

            if (_rutas?.Count > 0)
                _rutaSel = _rutas[0].IdRuta;

            await GetRutaParadas();
        }
    }
}
