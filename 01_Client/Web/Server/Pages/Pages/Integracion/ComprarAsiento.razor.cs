using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Infraestructura.Models.Integracion;
using Infraestructura.Abstract;

namespace Server.Pages.Pages.Integracion
{
    public partial class ComprarAsiento
    {
        // /comprar-asiento?viajeId=123&pax=2[&origenId=4&destinoId=2]
        [Parameter, SupplyParameterFromQuery] public int viajeId { get; set; }
        [Parameter, SupplyParameterFromQuery] public int pax { get; set; } = 1;
        [Parameter, SupplyParameterFromQuery] public int origenId { get; set; }   // opcional
        [Parameter, SupplyParameterFromQuery] public int destinoId { get; set; }  // opcional

        protected string _titulo;

        // horarios del día / ruta
        protected List<HorarioView> _horarios = new();
        protected int _viajeSeleccionadoId;
        protected string _horaSalidaDisplay = "";

        // seatmap
        protected List<SeatDto> _seatmap = new();
        protected Dictionary<int, SeatDto> _byNum = new();
        protected int _capacity = 37;
        protected int _mainRows = 8;
        protected int _lastRowCount = 5;

        // selección & reserva
        protected HashSet<int> _seleccion = new();
        protected int? _boletoId;
        protected DateTime? _venceUtc;
        protected TimeSpan _restante = TimeSpan.Zero;
        private readonly TimeSpan _ttlReserva = TimeSpan.FromMinutes(10);
        private Timer _timer;

        // habilita Pagar si ya hay reserva o si la selección alcanza pax
        protected bool BtnPagarHabilitado => _boletoId.HasValue || _seleccion.Count == pax;

        protected double _progresoPct =>
            !_boletoId.HasValue || !_venceUtc.HasValue
                ? 0
                : Math.Clamp((_restante.TotalSeconds / _ttlReserva.TotalSeconds) * 100.0, 0, 100);

        private bool LimiteAlcanzado => _seleccion.Count >= pax && !_boletoId.HasValue;

        protected override async Task OnInitializedAsync()
        {
            if (viajeId <= 0)
            {
                Snackbar.Add("Falta el identificador del viaje.", Severity.Error);
                Nav.NavigateTo("/comprar-public");
                return;
            }
            if (pax <= 0) pax = 1;

            await ResolverParadasHorariosYEncabezado();
            await CargarSeatmap(clearSelection: true);
            CalcularLayout();
        }

        // --------- Paradas, horarios y título ----------
        private async Task ResolverParadasHorariosYEncabezado()
        {
            string origen = "Origen", destino = "Destino";
            DateTime? fechaViaje = null;
            int? idRuta = null;

            // 1) Traer viajes y ubicar el actual
            var resViajes = await _Rest.GetAsync<List<ViajeApiDto>>("Viaje/viaje");
            var v = resViajes.State == State.Success ? resViajes.Data?.FirstOrDefault(x => x.IdViaje == viajeId) : null;
            if (v != null)
            {
                fechaViaje = v.Fecha;
                idRuta = v.IdRuta;
                _horaSalidaDisplay = (v.HoraSalida ?? "").Trim();
            }
            _viajeSeleccionadoId = viajeId;

            // 2) Intento #1: por ruta (preferido)
            if ((origenId <= 0 || destinoId <= 0) && idRuta.HasValue)
            {
                var resRP = await _Rest.GetAsync<List<RutaParadaDto>>($"RutaParada/GetAllIdRutaParada?idRuta={idRuta.Value}");
                if (resRP.State == State.Success && resRP.Data?.Any() == true)
                {
                    var ord = resRP.Data.OrderBy(p => p.Orden).ToList();
                    var pOri = ord.First();
                    var pDes = ord.Last();
                    if (origenId <= 0) origenId = pOri.Parada?.IdParada ?? 0;
                    if (destinoId <= 0) destinoId = pDes.Parada?.IdParada ?? 0;
                    origen = pOri.Parada?.Nombre ?? origen;
                    destino = pDes.Parada?.Nombre ?? destino;
                }
            }

            // 3) Intento #2: paradas del viaje
            if (origenId <= 0 || destinoId <= 0)
            {
                var resPar = await _Rest.GetAsync<List<RutaParadaDto>>($"Viaje/{viajeId}/paradas");
                if (resPar.State == State.Success && resPar.Data?.Any() == true)
                {
                    var ord = resPar.Data.OrderBy(p => p.Orden).ToList();
                    var pOri = ord.First(); var pDes = ord.Last();
                    if (origenId <= 0) origenId = pOri.Parada?.IdParada ?? 0;
                    if (destinoId <= 0) destinoId = pDes.Parada?.IdParada ?? 0;
                    origen = pOri.Parada?.Nombre ?? origen;
                    destino = pDes.Parada?.Nombre ?? destino;
                }
            }

            // 4) Intento #3: nombres desde Ruta/ruta (para el título, por si falló todo lo demás)
            if ((string.Equals(origen, "Origen", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(destino, "Destino", StringComparison.OrdinalIgnoreCase)) && idRuta.HasValue)
            {
                var resRutas = await _Rest.GetAsync<List<RutaDto>>("Ruta/ruta");
                var rut = resRutas.State == State.Success ? resRutas.Data?.FirstOrDefault(r => r.IdRuta == idRuta.Value) : null;
                if (rut != null)
                {
                    origen = string.IsNullOrWhiteSpace(origen) || origen == "Origen" ? rut.Origen : origen;
                    destino = string.IsNullOrWhiteSpace(destino) || destino == "Destino" ? rut.Destino : destino;
                }
            }

            // Horarios (chips)
            _horarios.Clear();
            var resViajes2 = await _Rest.GetAsync<List<ViajeApiDto>>("Viaje/viaje");
            if (resViajes2.State == State.Success && resViajes2.Data is not null && idRuta.HasValue && fechaViaje.HasValue)
            {
                var same = resViajes2.Data
                    .Where(x => x.IdRuta == idRuta.Value &&
                                x.Fecha.Date == fechaViaje.Value.Date &&
                                string.Equals(x.Estado, "PROGRAMADO", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(x => x.HoraSalida)
                    .ToList();

                _horarios = same.Select(x => new HorarioView
                {
                    IdViaje = x.IdViaje,
                    Hora = (x.HoraSalida ?? "").Trim()
                }).ToList();

                var match = same.FirstOrDefault(x => x.IdViaje == viajeId);
                if (match != null) _horaSalidaDisplay = (match.HoraSalida ?? "").Trim();
            }

            var horaTxt = string.IsNullOrWhiteSpace(_horaSalidaDisplay) ? "" : $" — {_horaSalidaDisplay}";
            _titulo = $"Asientos disponibles: {origen} → {destino}{horaTxt} para la fecha {(fechaViaje ?? DateTime.Today):dd-MM-yyyy}";
        }

        // cambiar horario
        protected async Task SeleccionarHorario(int nuevoViajeId)
        {
            if (nuevoViajeId == _viajeSeleccionadoId) return;

            _viajeSeleccionadoId = nuevoViajeId;
            viajeId = nuevoViajeId;

            // limpiar selección / temporizador / reserva
            _timer?.Stop();
            _boletoId = null;
            _venceUtc = null;
            _restante = TimeSpan.Zero;
            _seleccion.Clear();

            await ResolverParadasHorariosYEncabezado();
            await CargarSeatmap(clearSelection: true);
            CalcularLayout();

            StateHasChanged();
        }

        // --------- Normalización de estado ----------
        // Devuelve exactamente: LIBRE | BLOQUEADO | RESERVADO | OCUPADO
        private static string NormalizeEstado(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "LIBRE";
            var s = raw.Trim().ToUpperInvariant();

            if (s is "LIBRE") return "LIBRE";
            if (s is "OCUPADO") return "OCUPADO";

            if (s is "BLOQUEADO" or "BLOQUEADO TEMPORALMENTE" or "BLOQUEADO_TEMPORALMENTE" or "BLOQUEADO TEMP")
                return "BLOQUEADO";

            if (s is "RESERVADO" or "RESERVA")
                return "RESERVADO";

            // fallback seguro
            return "LIBRE";
        }

        // --------- Seatmap ----------
        private async Task CargarSeatmap(bool clearSelection)
        {
            // Construimos el URL SOLO con los parámetros que tengamos
            var qs = new List<string> { $"reservaTtlMinutos={(int)_ttlReserva.TotalMinutes}" };
            if (origenId > 0) qs.Add($"origenId={origenId}");
            if (destinoId > 0) qs.Add($"destinoId={destinoId}");

            var url = $"Viaje/{viajeId}/seatmap";
            if (qs.Count > 0) url += "?" + string.Join("&", qs);

            var resp = await _Rest.GetAsync<JsonElement>(url);
            if (resp.State != State.Success)
            {
                Snackbar.Add($"No fue posible cargar el seatmap: {resp.Message}", Severity.Error);
                return;
            }
            if (resp.Data.ValueKind == JsonValueKind.Undefined || resp.Data.ValueKind == JsonValueKind.Null)
            {
                Snackbar.Add("No fue posible cargar el seatmap: respuesta vacía.", Severity.Error);
                return;
            }

            try
            {
                List<SeatDto> seats = null!;
                int capacidad = 0;
                var data = resp.Data;

                if (data.ValueKind == JsonValueKind.Array)
                {
                    seats = JsonSerializer.Deserialize<List<SeatDto>>(data.GetRawText(),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<SeatDto>();
                }
                else if (data.ValueKind == JsonValueKind.Object)
                {
                    if (data.TryGetProperty("capacidad", out var capEl) && capEl.TryGetInt32(out var cap))
                        capacidad = cap;

                    if (data.TryGetProperty("asientos", out var seatsEl) ||
                        data.TryGetProperty("seats", out seatsEl) ||
                        data.TryGetProperty("lista", out seatsEl))
                    {
                        if (seatsEl.ValueKind == JsonValueKind.Array)
                        {
                            seats = JsonSerializer.Deserialize<List<SeatDto>>(seatsEl.GetRawText(),
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<SeatDto>();
                        }
                    }
                }

                if (seats == null)
                {
                    Snackbar.Add("Seatmap con formato inesperado.", Severity.Error);
                    return;
                }

                _seatmap = seats.OrderBy(s => s.Numero).ToList();
                _byNum = _seatmap.ToDictionary(s => s.Numero);
                _capacity = (capacidad > 0) ? capacidad : (_seatmap.Count > 0 ? _seatmap.Max(s => s.Numero) : 37);

                if (clearSelection) _seleccion.Clear();
                else
                {
                    var libres = _seatmap
                        .Where(s => NormalizeEstado(s.Estado) == "LIBRE")
                        .Select(s => s.Numero)
                        .ToHashSet();

                    _seleccion.RemoveWhere(n => !libres.Contains(n));
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Seatmap inválido: {ex.Message}", Severity.Error);
                return;
            }
        }

        // --------- Layout (37/41 o genérico) ----------
        private void CalcularLayout()
        {
            if (_capacity is 37 or 41)
            {
                _lastRowCount = 5;
                _mainRows = (_capacity - _lastRowCount) / 4;
            }
            else
            {
                _mainRows = Math.Max(0, _capacity / 4);
                _lastRowCount = _capacity - (_mainRows * 4);
                if (_lastRowCount == 0 && _mainRows > 0) { _mainRows -= 1; _lastRowCount = 5; }
                _lastRowCount = Math.Min(_lastRowCount, 5);
            }
        }

        // --------- UI ----------
        protected SeatDto GetSeat(int numero) => _byNum.TryGetValue(numero, out var s) ? s : null;

        private bool EsSeleccionable(SeatDto s)
            => !_boletoId.HasValue && NormalizeEstado(s.Estado) == "LIBRE";

        protected string SeatCss(SeatDto s)
        {
            var seleccionado = _seleccion.Contains(s.Numero);
            var estado = NormalizeEstado(s.Estado);
            var baseCss = "pasajes-public-asiento";
            if (seleccionado) return $"{baseCss} seleccionado";

            return estado switch
            {
                "LIBRE" => LimiteAlcanzado ? $"{baseCss} libre limitado" : $"{baseCss} libre",
                "OCUPADO" => $"{baseCss} ocupado",
                "BLOQUEADO" => $"{baseCss} bloqueado",
                "RESERVADO" => $"{baseCss} reservado",
                _ => $"{baseCss} libre"
            };
        }

        protected async Task Toggle(SeatDto s)
        {
            if (!EsSeleccionable(s)) return;

            if (_seleccion.Contains(s.Numero))
            {
                _seleccion.Remove(s.Numero);
            }
            else
            {
                if (LimiteAlcanzado)
                {
                    Snackbar.Add($"Ya seleccionaste {pax} asiento(s).", Severity.Info);
                    return;
                }

                // Revalidación instantánea del estado del asiento
                await CargarSeatmap(clearSelection: false);
                if (!(GetSeat(s.Numero) is SeatDto live && EsSeleccionable(live)))
                {
                    Snackbar.Add("El asiento ya no está disponible.", Severity.Warning);
                    return;
                }

                _seleccion.Add(s.Numero);

                // Auto-reserva cuando completa la cantidad
                if (_seleccion.Count == pax && !_boletoId.HasValue)
                    await Reservar();
            }
        }

        private async Task Reservar()
        {
            if (_seleccion.Count != pax)
            {
                Snackbar.Add($"Selecciona exactamente {pax} asiento(s).", Severity.Warning);
                return;
            }

            await CargarSeatmap(clearSelection: false);
            if (!_seleccion.All(n => GetSeat(n) is SeatDto seat && EsSeleccionable(seat)))
            {
                Snackbar.Add("Alguno de los asientos fue tomado. Vuelve a seleccionar.", Severity.Warning);
                return;
            }

            var payload = new Dictionary<string, object>
            {
                ["viajeId"] = viajeId,
                ["asientos"] = _seleccion.OrderBy(x => x).ToArray(),
                ["minutos"] = (int)_ttlReserva.TotalMinutes
            };
            if (origenId > 0) payload["origenId"] = origenId;
            if (destinoId > 0) payload["destinoId"] = destinoId;

            var res = await _Rest.PostAsync<BoletoReservaResp>("Boleto/reservar", payload);
            if (res.State != State.Success || res.Data is null)
            {
                var msg = string.IsNullOrWhiteSpace(res.Message) ? "Error desconocido" : res.Message;
                Snackbar.Add($"No se pudo reservar los asientos: {msg}", Severity.Error);
                return;
            }

            _boletoId = res.Data.BoletoId;
            _venceUtc = res.Data.Vence == default ? DateTime.UtcNow.Add(_ttlReserva) : res.Data.Vence;
            IniciarTimer();
            Snackbar.Add($"Asientos reservados: {string.Join(", ", _seleccion.OrderBy(x => x))}.", Severity.Success);
        }

        private void IniciarTimer()
        {
            if (!_venceUtc.HasValue) return;

            _timer?.Stop();
            _timer = new Timer(1000);
            _timer.Elapsed += (_, __) =>
            {
                _restante = _venceUtc.Value - DateTime.UtcNow;
                if (_restante <= TimeSpan.Zero)
                {
                    _restante = TimeSpan.Zero;
                    _timer.Stop();
                    _boletoId = null; // expiró: obliga a reservar de nuevo
                    InvokeAsync(() => Snackbar.Add("La reserva expiró. Selecciona de nuevo los asientos.", Severity.Info));
                }
                InvokeAsync(StateHasChanged);
            };
            _timer.Start();
        }

        protected async Task IrAPagar()
        {
            // Si no hay reserva pero ya está completa la selección, reserva ahora
            if (!_boletoId.HasValue)
            {
                if (_seleccion.Count != pax)
                {
                    Snackbar.Add($"Selecciona {pax} asiento(s) para continuar.", Severity.Info);
                    return;
                }

                await Reservar();
                if (!_boletoId.HasValue) return; // si falló, no avances
            }

            Nav.NavigateTo($"/comprar-datos?boletoId={_boletoId.Value}");
        }

        protected string WhatsAppLink()
        {
            var seats = string.Join(", ", _seleccion.OrderBy(x => x));
            var txt = Uri.EscapeDataString($"Hola, deseo reservar mis pasajes. Viaje #{viajeId}. Asientos: {seats}.");
            return $"https://wa.me/591XXXXXXXX?text={txt}";
        }

        // --------- DTOs ----------
        public class SeatDto
        {
            public int Numero { get; set; }                 // 1..37/41
            public string Estado { get; set; } = "LIBRE";   // LIBRE | BLOQUEADO | RESERVADO | OCUPADO
        }

        public class RutaParadaDto
        {
            public int IdRutaParada { get; set; }
            public int IdRuta { get; set; }
            public int IdParada { get; set; }
            public int Orden { get; set; }
            public ParadaDto Parada { get; set; }
        }
        public class ParadaDto { public int IdParada { get; set; } public string Nombre { get; set; } }

        public class ViajeApiDto
        {
            public int IdViaje { get; set; }
            public DateTime Fecha { get; set; }
            public string HoraSalida { get; set; }
            public string Estado { get; set; }
            public int IdRuta { get; set; }
        }

        public class BoletoReservaResp
        {
            public int BoletoId { get; set; }
            public DateTime Vence { get; set; } // UTC
            public decimal Total { get; set; }
            public string Moneda { get; set; }
        }

        public class HorarioView
        {
            public int IdViaje { get; set; }
            public string Hora { get; set; }
        }
    }
}
