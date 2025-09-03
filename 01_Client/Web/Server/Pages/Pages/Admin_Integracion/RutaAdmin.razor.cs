using Infraestructura.Abstract;
using Infraestructura.Models.Integracion;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server.Pages.Pages.Admin_Integracion
{
    public partial class RutaAdmin
    {
        private bool expande = false;
        private bool _isSubmitting = false;

        public RutaDto _Ruta = new();
        public List<RutaDto> _rutas = new();

        // ---------- CRUD ----------
        private async Task GetRutas()
        {
            try
            {
                var res = await _Rest.GetAsync<List<RutaDto>>("Ruta/ruta");
                if (res.State == State.Success)
                    _rutas = res.Data ?? new List<RutaDto>();
                else
                    _MessageShow(res.Message, State.Warning);
            }
            catch (Exception ex)
            {
                _MessageShow($"No se pudo obtener rutas: {ex.Message}", State.Error);
            }
        }

        private async Task Save(RutaDto dto)
        {
            try
            {
                _Loading.Show();
                var r = await _Rest.PostAsync<int?>("Ruta/guardar", new { Ruta = dto });
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

        private async Task Update(RutaDto dto)
        {
            try
            {
                _Loading.Show();
                var r = await _Rest.PutAsync<int>("Ruta", dto, dto.IdRuta);
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

        private async Task Eliminar(int id)
        {
            await _MessageConfirm("¿Eliminar la ruta?", async () =>
            {
                try
                {
                    var r = await _Rest.DeleteAsync<int>("Ruta", id);
                    if (!r.Succeeded)
                        _MessageShow(r.Message, State.Error);
                    else
                    {
                        _MessageShow(r.Message, r.State);
                        await GetRutas();
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
        private async Task OnValidRuta(EditContext _)
        {
            if (_isSubmitting) return;
            _isSubmitting = true;

            try
            {
                if (_Ruta.IdRuta > 0)
                    await Update(_Ruta);
                else
                    await Save(_Ruta);

                _Ruta = new RutaDto();
                await GetRutas();
                ToggleExpand();
            }
            finally
            {
                _isSubmitting = false;
            }
        }

        private void FormEditar(RutaDto dto)
        {
            _Ruta = new RutaDto
            {
                IdRuta = dto.IdRuta,
                Origen = dto.Origen,
                Destino = dto.Destino,
                Duracion = dto.Duracion,
                EsExtendido = dto.EsExtendido
            };
            ToggleExpand();
        }

        private void ResetRuta() => _Ruta = new RutaDto();
        private void ToggleExpand() => expande = !expande;

        // ---------- Init ----------
        protected override async Task OnInitializedAsync()
        {
            await GetRutas();
        }
    }
}
