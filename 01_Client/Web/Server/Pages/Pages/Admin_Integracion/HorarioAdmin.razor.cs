using Infraestructura.Abstract;
using Infraestructura.Models.Integracion;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization; // <-- agregado

namespace Server.Pages.Pages.Admin_Integracion
{
    public partial class HorarioAdmin
    {
        private bool expande = false;
        private bool _isSubmitting = false;

        public HorarioDto _Horario = new();
        public List<HorarioDto> _horarios = new();
        public List<RutaDto> _rutas = new();
        public List<ParadaDto> _paradas = new();

        // Máscara HH:mm (IMask de MudBlazor)
        protected readonly PatternMask _maskHora = new("00:00");

        // Días de la semana
        protected static readonly string[] _dias = new[] { "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb", "Dom" };

        // Wrapper para ruta seleccionada
        protected int IdRutaSel
        {
            get => _Horario.IdRuta;
            set
            {
                if (_Horario.IdRuta != value)
                {
                    _Horario.IdRuta = value;
                    _ = OnRutaChanged(value);
                }
            }
        }

        // ---------- Helpers ----------
        protected string NombreRuta(int idRuta)
        {
            var r = _rutas.FirstOrDefault(x => x.IdRuta == idRuta);
            return r is null ? $"Ruta {idRuta}" : $"{r.Origen} → {r.Destino}";
        }

        protected string NombreParada(int? idParada)
        {
            if (idParada is null) return "-";
            var p = _paradas.FirstOrDefault(x => x.IdParada == idParada.Value);
            return p is null ? $"Parada {idParada}" : p.Nombre;
        }

        protected async Task OnRutaChanged(int idRuta)
        {
            await GetParadas(); // si el backend filtra por ruta, úsalo aquí
        }

        // ---------- Normalización ----------
        private static string NormalizarHora(string h)
        {
            if (string.IsNullOrWhiteSpace(h)) return "00:00:00";
            // Acepta HH:mm o HH:mm:ss y devuelve HH:mm:ss
            return TimeSpan.TryParseExact(h,
                                          new[] { @"hh\:mm", @"hh\:mm\:ss" },
                                          CultureInfo.InvariantCulture,
                                          out var ts)
                ? ts.ToString(@"hh\:mm\:ss")
                : "00:00:00";
        }

        private static string NormalizarDireccion(string d)
            => string.IsNullOrWhiteSpace(d) ? "IDA" : d.ToUpperInvariant();

        // ---------- CRUD ----------
        private async Task GetHorarios()
        {
            try
            {
                var res = await _Rest.GetAsync<List<HorarioDto>>("Horario/horario");
                _horarios = res.State == State.Success ? (res.Data ?? new()) : new();
                if (res.State != State.Success) _MessageShow(res.Message, State.Warning);
            }
            catch (Exception ex)
            {
                _MessageShow($"No se pudo obtener horarios: {ex.Message}", State.Error);
                _horarios = new();
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

        private async Task Save(HorarioDto dto)
        {
            try
            {
                _Loading.Show();

                // 👇 Normalizaciones antes de enviar al backend
                dto.HoraSalida = NormalizarHora(dto.HoraSalida);
                dto.Direccion = NormalizarDireccion(dto.Direccion);

                var r = await _Rest.PostAsync<int?>("Horario/guardar", new { Horario = dto });
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

        private async Task Update(HorarioDto dto)
        {
            try
            {
                _Loading.Show();

                // 👇 Normalizaciones también para update
                dto.HoraSalida = NormalizarHora(dto.HoraSalida);
                dto.Direccion = NormalizarDireccion(dto.Direccion);

                var r = await _Rest.PutAsync<int>("Horario", dto, dto.IdHorario);
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

        protected async Task Eliminar(int id)
        {
            await _MessageConfirm("¿Eliminar el horario?", async () =>
            {
                try
                {
                    var r = await _Rest.DeleteAsync<int>("Horario", id);
                    if (!r.Succeeded)
                        _MessageShow(r.Message, State.Error);
                    else
                    {
                        _MessageShow(r.Message, r.State);
                        await GetHorarios();
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
        private async Task OnValidHorario(EditContext _)
        {
            if (_isSubmitting) return;
            _isSubmitting = true;

            try
            {
                if (_Horario.DesdeParadaId.HasValue &&
                    _Horario.HastaParadaId.HasValue &&
                    _Horario.DesdeParadaId.Value == _Horario.HastaParadaId.Value)
                {
                    _MessageShow("La parada 'Desde' y 'Hasta' no pueden ser la misma.", State.Warning);
                    return;
                }

                if (_Horario.IdHorario > 0)
                    await Update(_Horario);
                else
                    await Save(_Horario);

                var rutaSel = _Horario.IdRuta;
                _Horario = new HorarioDto
                {
                    IdRuta = rutaSel,
                    HoraSalida = "00:00", // se normaliza a HH:mm:ss antes de enviar
                    DiaSemana = "Lun",
                    Direccion = "IDA"
                };

                await GetHorarios();
                ToggleExpand();
            }
            finally
            {
                _isSubmitting = false;
            }
        }

        protected void FormEditar(HorarioDto dto)
        {
            _Horario = new HorarioDto
            {
                IdHorario = dto.IdHorario,
                IdRuta = dto.IdRuta,
                HoraSalida = dto.HoraSalida,
                DiaSemana = dto.DiaSemana,
                Direccion = dto.Direccion,
                DesdeParadaId = dto.DesdeParadaId,
                HastaParadaId = dto.HastaParadaId
            };
            ToggleExpand();
        }

        protected void ResetHorario()
        {
            var rutaSel = _Horario.IdRuta;
            _Horario = new HorarioDto
            {
                IdRuta = rutaSel,
                HoraSalida = "00:00",
                DiaSemana = "Lun",
                Direccion = "IDA"
            };
        }

        protected void ToggleExpand() => expande = !expande;

        // ---------- Init ----------
        protected override async Task OnInitializedAsync()
        {
            await GetRutas();
            await GetParadas();
            await GetHorarios();
        }
    }
}
