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
    public partial class EncomiendaAdmin
    {
        private bool expande = false;
        private bool _isSubmitting = false;

        // Estados permitidos (para el MudSelect)
        private static readonly string[] _estados = new[]
        {
            "en camino", "en destino", "entregado", "devuelta"
        };

        // Filtro
        private string? _filtroGuiacarga;

        // Form
        public EncomiendaDto _E = new();

        // Listas
        public List<EncomiendaDto> _encomiendas = new();
        public List<ViajeDto> _viajes = new();
        public List<ParadaDto> _paradas = new();
        public List<RutaDto> _rutas = new();

        // ===== Helpers de UI =====
        private void ToggleExpand() => expande = !expande;

        private void ResetForm()
        {
            _E = new EncomiendaDto
            {
                Estado = "en camino",
                Pagado = false
            };
        }

        private string ViajeEtiqueta(ViajeDto v)
        {
            var ruta = _rutas.FirstOrDefault(r => r.IdRuta == v.IdRuta);
            var rname = ruta is null ? $"Ruta {v.IdRuta}" : $"{ruta.Origen} → {ruta.Destino}";
            return $"{v.Fecha:dd/MM/yyyy} {v.HoraSalida} · {rname}";
        }

        private IEnumerable<ParadaDto> ParadasFiltradasParaViaje(int idViaje)
        {
            var v = _viajes.FirstOrDefault(x => x.IdViaje == idViaje);
            if (v == null) return _paradas; // fallback: todas
            // Si tienes "ruta_parada" en el front podrías filtrar por v.IdRuta. Por ahora devolvemos todas.
            return _paradas;
        }

        private string FechaViaje(EncomiendaDto e)
        {
            var v = _viajes.FirstOrDefault(x => x.IdViaje == e.IdViaje);
            return v is null ? "-" : v.Fecha.ToString("dd/MM/yyyy");
        }

        private string HoraViaje(EncomiendaDto e)
        {
            var v = _viajes.FirstOrDefault(x => x.IdViaje == e.IdViaje);
            return v?.HoraSalida ?? "-";
        }

        private string RutaNombrePorViaje(int idViaje)
        {
            var v = _viajes.FirstOrDefault(x => x.IdViaje == idViaje);
            if (v == null) return "-";
            var r = _rutas.FirstOrDefault(x => x.IdRuta == v.IdRuta);
            return r is null ? $"Ruta {v.IdRuta}" : $"{r.Origen} → {r.Destino}";
        }

        // ===== CRUD =====
        private async Task GetEncomiendas()
        {
            try
            {
                var url = string.IsNullOrWhiteSpace(_filtroGuiacarga)
                    ? "Encomienda/encomienda"
                    : $"Encomienda/encomienda?guiacarga={Uri.EscapeDataString(_filtroGuiacarga)}";

                var res = await _Rest.GetAsync<List<EncomiendaDto>>(url);
                _encomiendas = res.State == State.Success ? (res.Data ?? new()) : new();
                if (res.State != State.Success) _MessageShow(res.Message, State.Warning);
            }
            catch (Exception ex)
            {
                _MessageShow($"No se pudo obtener encomiendas: {ex.Message}", State.Error);
                _encomiendas = new();
            }
        }

        private async Task GetViajes()
        {
            try
            {
                var res = await _Rest.GetAsync<List<ViajeDto>>("Viaje/viaje");
                _viajes = res.State == State.Success ? (res.Data ?? new()) : new();
                if (res.State != State.Success) _MessageShow(res.Message, State.Warning);
            }
            catch (Exception ex)
            {
                _MessageShow($"No se pudo obtener viajes: {ex.Message}", State.Error);
                _viajes = new();
            }
        }

        private async Task GetParadas()
        {
            try
            {
                var res = await _Rest.GetAsync<List<ParadaDto>>("Parada/parada");
                _paradas = res.State == State.Success ? (res.Data ?? new()) : new();
                if (res.State != State.Success) _MessageShow(res.Message, State.Warning);
            }
            catch (Exception ex)
            {
                _MessageShow($"No se pudo obtener paradas: {ex.Message}", State.Error);
                _paradas = new();
            }
        }

        private async Task GetRutas()
        {
            try
            {
                var res = await _Rest.GetAsync<List<RutaDto>>("Ruta/ruta");
                _rutas = res.State == State.Success ? (res.Data ?? new()) : new();
                if (res.State != State.Success) _MessageShow(res.Message, State.Warning);
            }
            catch (Exception ex)
            {
                _MessageShow($"No se pudo obtener rutas: {ex.Message}", State.Error);
                _rutas = new();
            }
        }

        private async Task Save(EncomiendaDto dto)
        {
            try
            {
                _Loading.Show();
                var r = await _Rest.PostAsync<int?>("Encomienda/guardar", new { Encomienda = dto });
                _MessageShow(r.Message, r.State);

                if (r.State != State.Success && r.Errors != null)
                    foreach (var e in r.Errors) _MessageShow(e, State.Warning);
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

        private async Task Update(EncomiendaDto dto)
        {
            try
            {
                _Loading.Show();
                var r = await _Rest.PutAsync<int>("Encomienda", dto, dto.IdEncomienda);
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
            await _MessageConfirm("¿Eliminar la encomienda?", async () =>
            {
                try
                {
                    var r = await _Rest.DeleteAsync<int>("Encomienda", id);
                    if (!r.Succeeded) _MessageShow(r.Message, State.Error);
                    else
                    {
                        _MessageShow(r.Message, r.State);
                        await GetEncomiendas();
                        StateHasChanged();
                    }
                }
                catch (Exception ex)
                {
                    _MessageShow($"Error al eliminar: {ex.Message}", State.Error);
                }
            });
        }

        private async Task OnValidSubmit(EditContext _)
        {
            if (_isSubmitting) return;
            _isSubmitting = true;

            try
            {
                if (_E.IdViaje <= 0)
                {
                    _MessageShow("Debe seleccionar un Viaje.", State.Warning);
                    return;
                }

                if (_E.IdEncomienda > 0) await Update(_E);
                else await Save(_E);

                ResetForm();
                await GetEncomiendas();
                if (expande) ToggleExpand();
            }
            finally
            {
                _isSubmitting = false;
            }
        }

        private void Editar(EncomiendaDto dto)
        {
            _E = new EncomiendaDto
            {
                IdEncomienda = dto.IdEncomienda,
                Remitente = dto.Remitente,
                Destinatario = dto.Destinatario,
                Guiacarga = dto.Guiacarga,
                Descripcion = dto.Descripcion,
                Peso = dto.Peso,
                Precio = dto.Precio,
                IdViaje = dto.IdViaje,
                Estado = dto.Estado,
                Pagado = dto.Pagado,
                OrigenParadaId = dto.OrigenParadaId,
                DestinoParadaId = dto.DestinoParadaId
            };

            if (!expande) ToggleExpand();
        }

        // ===== Init =====
        protected override async Task OnInitializedAsync()
        {
            ResetForm();
            await GetRutas();
            await GetParadas();
            await GetViajes();
            await GetEncomiendas();
        }
    }
}
