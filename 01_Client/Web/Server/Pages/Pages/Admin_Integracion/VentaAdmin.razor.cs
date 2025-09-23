using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Infraestructura.Abstract;
using Infraestructura.Models.Integracion;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Server.Shared.Component;

namespace Server.Pages.Pages.Admin_Integracion
{
    public partial class VentaAdmin : MainBaseComponent
    {
        // ===================== Endpoints base existentes =====================
        private const string EP_VIAJES = "Viaje/viaje";
        private const string EP_RUTAS = "Ruta/ruta";
        private const string EP_CHOFER = "Chofer/chofer";
        private const string EP_BUS = "Bus/bus";

        // Nuevos/auxiliares
        private static string EP_VIAJE_PARADAS(int idViaje) => $"Viaje/{idViaje}/paradas";
        private static string EP_VIAJE_SEATMAP(int idViaje, int origenId, int destinoId)
            => $"Viaje/{idViaje}/seatmap?origenId={origenId}&destinoId={destinoId}";

        private const string EP_BOLETO_RESERVAR = "Boleto/reservar";
        private const string EP_BOLETO_CONFIRMAR = "Boleto/confirmar";
        private const string EP_BOLETO_REPROGRAMAR = "Boleto/reprogramar";
        private static string EP_BOLETO_DELETE(int idBoleto) => $"Boleto/{idBoleto}";

        // ===================== Filtros superiores =====================
        private DateTime? _selectedDate = DateTime.Today;
        protected DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                var nv = value ?? DateTime.Today;
                if (_selectedDate != nv)
                {
                    _selectedDate = nv;
                    _ = CargarPlanilla();
                }
            }
        }
        protected int _diasFiltro = 1;

        // ===================== Planilla =====================
        protected bool IsLoading { get; set; }
        protected List<ViajePlanillaItem> _planilla = new();

        // Estado expandido por viaje (data + UI)
        private readonly Dictionary<int, RowUiState> _ui = new();

        // Id del viaje actualmente abierto (para el botón "Vender/Ver" -> "Cerrar")
        protected int? _viajeAbiertoId;

        // ===================== Ciclo de vida =====================
        protected override async Task OnInitializedAsync() => await CargarPlanilla();
        protected async Task SetHoy() { SelectedDate = DateTime.Today; await CargarPlanilla(); }
        protected async Task SetManana() { SelectedDate = DateTime.Today.AddDays(1); await CargarPlanilla(); }

        // ===================== Cargar planilla =====================
        protected async Task CargarPlanilla()
        {
            IsLoading = true;
            _planilla = new();
            try
            {
                var fecha = (SelectedDate ?? DateTime.Today).Date;
                var desde = fecha;
                var hasta = fecha.AddDays(Math.Max(1, _diasFiltro) - 1);

                var rv = await _Rest.GetAsync<List<ViajeDto>>(EP_VIAJES);
                var rr = await _Rest.GetAsync<List<RutaDto>>(EP_RUTAS);
                var rc = await _Rest.GetAsync<List<ChoferDto>>(EP_CHOFER);
                var rb = await _Rest.GetAsync<List<BusDto>>(EP_BUS);

                var viajes = rv.State == State.Success && rv.Data != null ? rv.Data : new();
                var rutas = rr.State == State.Success && rr.Data != null ? rr.Data : new();
                var chof = rc.State == State.Success && rc.Data != null ? rc.Data : new();
                var buses = rb.State == State.Success && rb.Data != null ? rb.Data : new();

                var viajesRango = viajes.Where(v => v.Fecha.Date >= desde && v.Fecha.Date <= hasta)
                                        .OrderBy(v => v.Fecha).ThenBy(v => v.HoraSalida)
                                        .ToList();

                _planilla = viajesRango.Select(v =>
                {
                    var ruta = rutas.FirstOrDefault(x => x.IdRuta == v.IdRuta);
                    var ch = chof.FirstOrDefault(x => x.IdChofer == v.IdChofer);
                    var bus = buses.FirstOrDefault(x => x.IdBus == v.IdBus);

                    return new ViajePlanillaItem
                    {
                        Fecha = v.Fecha.Date,
                        IdViaje = v.IdViaje,
                        HoraSalida = v.HoraSalida,
                        RutaNombre = ruta is null ? $"Ruta {v.IdRuta}" : $"{ruta.Origen} → {ruta.Destino}",
                        IdBus = v.IdBus,
                        Placa = bus?.Placa ?? "-",
                        IdChofer = v.IdChofer,
                        ChoferNombre = ch?.Nombre ?? "-",
                        Capacidad = bus?.Capacidad ?? 0,
                        Ocupados = 0,
                        Estado = v.Estado ?? "PROGRAMADO",
                        CodigoViaje = v.IdViaje.ToString()
                    };
                }).ToList();

                // Mantener estado expandido existente (si aplica)
                var keep = _ui.Keys.ToHashSet();
                foreach (var id in keep)
                    if (!_planilla.Any(p => p.IdViaje == id)) _ui.Remove(id);
            }
            catch (Exception ex)
            {
                _MessageShow($"Error cargando planilla: {ex.Message}", State.Error);
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        protected bool PuedeVender(string? estado) =>
            (estado ?? "PROGRAMADO") is "PROGRAMADO" or "Embarcando" or "EMBARCANDO";

        protected Color EstadoColor(string? estado)
        {
            var e = (estado ?? "PROGRAMADO").ToUpperInvariant();
            return e switch
            {
                "PROGRAMADO" => Color.Info,
                "EMBARCANDO" => Color.Warning,
                "ENRUTA" => Color.Secondary,
                "FINALIZADO" => Color.Success,
                "CANCELADO" => Color.Error,
                _ => Color.Default
            };
        }

        // ===================== Expand / Collapse =====================
        protected async Task ToggleExpand(ViajePlanillaItem v)
        {
            // Si el mismo viaje ya está abierto -> cerrar
            if (_ui.ContainsKey(v.IdViaje))
            {
                _ui.Remove(v.IdViaje);
                if (_viajeAbiertoId == v.IdViaje) _viajeAbiertoId = null;
                StateHasChanged();
                return;
            }

            // Cerrar otro abierto (solo uno a la vez, si así lo prefieres)
            if (_viajeAbiertoId.HasValue && _ui.ContainsKey(_viajeAbiertoId.Value))
                _ui.Remove(_viajeAbiertoId.Value);

            _viajeAbiertoId = v.IdViaje;

            // crear estado por defecto y cargar paradas + seatmap
            var u = new RowUiState
            {
                Viaje = v,
                Loading = true,
                Cliente = new ClienteDto { IdCliente = 0, Nombre = "", Carnet = 0, Celular = 0 },
                MetodoPago = "EFECTIVO",
                PrecioUnitario = 0.00M,
                Seleccionados = new Dictionary<int, int>() // seatId -> boletoId
            };
            _ui[v.IdViaje] = u;

            try
            {
                // Paradas del viaje
                var r = await _Rest.GetAsync<List<ParadaDto>>(EP_VIAJE_PARADAS(v.IdViaje));
                u.Paradas = r.State == State.Success && r.Data != null ? r.Data : new List<ParadaDto>();
                if (u.Paradas.Count >= 2)
                {
                    var origenSucre = u.Paradas
                        .FirstOrDefault(p => string.Equals(p.Nombre?.Trim(), "Sucre", StringComparison.OrdinalIgnoreCase))?.IdParada
                        ?? u.Paradas.First().IdParada;

                    u.OrigenParadaId = origenSucre;
                    u.DestinoParadaId = u.Paradas.Last(p => p.IdParada != u.OrigenParadaId).IdParada;
                }

                // Cargar seatmap con el tramo (origen fijo + destino elegido)
                await CargarSeatmap(u);

            }
            catch (Exception ex)
            {
                _MessageShow($"Error al expandir: {ex.Message}", State.Error);
            }
            finally
            {
                u.Loading = false;
                StateHasChanged();
            }
        }

        // ===================== Seatmap por tramo =====================
        protected async Task CargarSeatmap(RowUiState u)
        {
            if (u.Viaje is null || u.OrigenParadaId == 0 || u.DestinoParadaId == 0 || u.OrigenParadaId == u.DestinoParadaId)
            {
                u.Seatmap = new();
                return;
            }

            u.Loading = true;
            try
            {
                var ep = EP_VIAJE_SEATMAP(u.Viaje.IdViaje, u.OrigenParadaId, u.DestinoParadaId);
                var rs = await _Rest.GetAsync<List<SeatmapSeatDto>>(ep);
                u.Seatmap = rs.State == State.Success && rs.Data != null ? rs.Data.OrderBy(s => s.Numero).ToList() : new();

                // Ocupación visible en fila
                var plan = _planilla.FirstOrDefault(x => x.IdViaje == u.Viaje.IdViaje);
                if (plan != null)
                    plan.Ocupados = u.Seatmap.Count(s => s.EstadoSeat is "OCUPADO" or "RESERVADO");
            }
            catch (Exception ex)
            {
                _MessageShow($"Error al cargar seatmap: {ex.Message}", State.Error);
            }
            finally
            {
                u.Loading = false;
                StateHasChanged();
            }
        }

        // ===================== Click de asiento =====================
        protected async Task OnSeatClick(RowUiState u, SeatmapSeatDto a)
        {
            if (u.Loading || a is null) return;

            switch (a.EstadoSeat?.ToUpperInvariant())
            {
                case "LIBRE":
                    await ReservarAsiento(u, a);
                    break;

                case "RESERVADO":
                case "OCUPADO":
                    // Aquí podrías abrir un panel o tooltip con info (a.ClienteNombre, CI, Precio, tramo)
                    // y exponer botón para reprogramar. Por ahora sólo mostramos un toast.
                    var tipo = a.EstadoSeat.ToUpperInvariant() == "OCUPADO" ? "ocupado" : "reservado";
                    _MessageShow($"Asiento {a.Numero} {tipo}. Cliente: {a.ClienteNombre ?? "-"}", State.Success);
                    break;
            }
        }

        // ===================== Reservar (selección) =====================
        private async Task ReservarAsiento(RowUiState u, SeatmapSeatDto a)
        {
            if (u.Viaje is null || a is null || a.EstadoSeat != "LIBRE") return;
            if (u.PrecioUnitario <= 0)
            {
                _MessageShow("Indica el precio antes de reservar.", State.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(u.Cliente?.Nombre))
            {
                _MessageShow("Registra el cliente (Nombre).", State.Warning);
                return;
            }

            try
            {
                u.Loading = true;

                int idCliente = await EnsureClienteId(u.Cliente);

                var body = new
                {
                    IdViaje = u.Viaje.IdViaje,
                    IdAsiento = a.IdAsiento,
                    IdCliente = idCliente,
                    OrigenParadaId = u.OrigenParadaId,
                    DestinoParadaId = u.DestinoParadaId,
                    Precio = u.PrecioUnitario
                };

                var r = await _Rest.PostAsync<int?>(EP_BOLETO_RESERVAR, body);
                if (r.State != State.Success || r.Data is null || r.Data.Value <= 0)
                {
                    _MessageShow(r.Message ?? "No se pudo reservar el asiento.", State.Warning);
                    return;
                }

                // Guardar la reserva localmente (seat -> boletoId)
                u.Seleccionados[a.IdAsiento] = r.Data.Value;
                _MessageShow($"Asiento {a.Numero} reservado.", State.Success);
            }
            catch (Exception ex)
            {
                _MessageShow($"Error al reservar: {ex.Message}", State.Error);
            }
            finally
            {
                u.Loading = false;
                await CargarSeatmap(u);
            }
        }

        // Des-seleccionar (eliminar BLOQUEADO propio)
        protected async Task QuitarSeleccion(RowUiState u, SeatmapSeatDto a)
        {
            if (a is null || !u.Seleccionados.TryGetValue(a.IdAsiento, out var idBoleto)) return;

            await _MessageConfirm($"¿Quitar selección del asiento {a.Numero}?", async () =>
            {
                var del = await _Rest.DeleteAsync<int>(EP_BOLETO_DELETE(idBoleto), idBoleto);
                if (!del.Succeeded)
                {
                    _MessageShow(del.Message ?? "No se pudo quitar la reserva.", State.Error);
                    return;
                }

                u.Seleccionados.Remove(a.IdAsiento);
                _MessageShow("Reserva quitada.", State.Success);
                await CargarSeatmap(u);
            });
        }

        // ===================== Confirmar múltiples =====================
        protected bool PuedeConfirmar(RowUiState u)
        {
            if (u is null) return false;
            var n = u.Seleccionados.Count;
            bool refOk = u.MetodoPago == "EFECTIVO" || !string.IsNullOrWhiteSpace(u.ReferenciaPago);
            return n > 0 && u.PrecioUnitario > 0 && !string.IsNullOrWhiteSpace(u.Cliente?.Nombre) && refOk;
        }

        protected async Task ConfirmarSeleccion(RowUiState u)
        {
            if (!PuedeConfirmar(u)) return;

            try
            {
                u.Loading = true;

                int idCliente = await EnsureClienteId(u.Cliente);

                var body = new
                {
                    BoletoIds = u.Seleccionados.Values.ToList(),
                    IdCliente = idCliente,
                    PrecioUnitario = u.PrecioUnitario,
                    MetodoPago = u.MetodoPago,
                    ReferenciaPago = u.MetodoPago is "QR" or "TRANSFERENCIA" ? u.ReferenciaPago : null
                };

                var r = await _Rest.PostAsync<ConfirmarBoletosResultDto>(EP_BOLETO_CONFIRMAR, body);
                if (r.State != State.Success || r.Data is null)
                {
                    _MessageShow(r.Message ?? "No se pudo confirmar.", State.Warning);
                    return;
                }

                var d = r.Data;
                if (d.Ok.Count > 0 && d.Fail.Count == 0)
                    _MessageShow($"Se emitieron {d.Ok.Count} boletos. Total Bs {d.TotalCobrado:0.00}", State.Success);
                else if (d.Ok.Count > 0)
                    _MessageShow($"Parcial: {d.Ok.Count} ok, {d.Fail.Count} con error.", State.Warning);
                else
                    _MessageShow("No se pudo emitir ningún boleto.", State.Warning);

                // Limpiar seleccionados confirmados
                var okSet = d.Ok.ToHashSet();
                u.Seleccionados = u.Seleccionados
                    .Where(kv => !okSet.Contains(kv.Value))
                    .ToDictionary(kv => kv.Key, kv => kv.Value);

                await CargarSeatmap(u);
                await CargarPlanilla();
            }
            catch (Exception ex)
            {
                _MessageShow($"Error al confirmar: {ex.Message}", State.Error);
            }
            finally
            {
                u.Loading = false;
            }
        }

        // ===================== Reprogramación =====================
        // (UI: elige viaje/asiento destino y opcionalmente tramo destino)
        protected async Task ReprogramarBoleto(int idBoleto, int idViajeDestino, int idAsientoDestino, int? origenDestinoId, int? destinoDestinoId, string motivo = "Reprogramación")
        {
            try
            {
                _Loading.Show();

                var body = new
                {
                    IdBoleto = idBoleto,
                    IdViajeDestino = idViajeDestino,
                    IdAsientoDestino = idAsientoDestino,
                    OrigenParadaId = origenDestinoId,
                    DestinoParadaId = destinoDestinoId,
                    Motivo = motivo,
                    IdUsuario = 1
                };

                var rs = await _Rest.PostAsync<int?>(EP_BOLETO_REPROGRAMAR, body);
                if (rs.State != State.Success)
                {
                    _MessageShow(rs.Message ?? "No se pudo reprogramar el boleto.", State.Warning);
                    return;
                }

                _MessageShow("Boleto reprogramado.", State.Success);
                // Refrescar vistas afectadas: planilla y cualquier viaje expandido
                await CargarPlanilla();
                foreach (var s in _ui.Values.ToList())
                    await CargarSeatmap(s);
            }
            catch (Exception ex)
            {
                _MessageShow($"Error al reprogramar: {ex.Message}", State.Error);
            }
            finally
            {
                _Loading.Hide();
            }
        }

        // ===================== Helpers =====================
        private async Task<int> EnsureClienteId(ClienteDto cli)
        {
            if (cli is null) throw new InvalidOperationException("Cliente requerido.");

            if (cli.IdCliente > 0) return cli.IdCliente;

            // Buscar posibles existentes por Carnet o por Nombre+Celular
            var rList = await _Rest.GetAsync<List<ClienteDto>>("Cliente/cliente");
            var lista = (rList.State == State.Success && rList.Data != null) ? rList.Data : new List<ClienteDto>();

            var existente = lista.FirstOrDefault(c =>
                (cli.Carnet != 0 && c.Carnet == cli.Carnet) ||
                (!string.IsNullOrWhiteSpace(cli.Nombre) &&
                 string.Equals((c.Nombre ?? "").Trim(), cli.Nombre.Trim(), StringComparison.OrdinalIgnoreCase) &&
                 c.Celular == cli.Celular));

            if (existente is not null) return existente.IdCliente;

            var nuevo = new ClienteDto
            {
                Nombre = cli.Nombre?.Trim() ?? "",
                Carnet = cli.Carnet,
                Celular = cli.Celular
            };

            var rSave = await _Rest.PostAsync<int?>("Cliente/guardar", new { Cliente = nuevo });
            if (rSave.State != State.Success || rSave.Data is null || rSave.Data.Value <= 0)
                throw new InvalidOperationException(rSave.Message ?? "No se pudo registrar el cliente.");

            return rSave.Data.Value;
        }

        // ===================== Modelos UI / DTOs locales =====================
        public class ViajePlanillaItem
        {
            public DateTime Fecha { get; set; }
            public int IdViaje { get; set; }
            public string HoraSalida { get; set; } = "";
            public string RutaNombre { get; set; } = "";
            public int IdBus { get; set; }
            public string Placa { get; set; } = "";
            public int IdChofer { get; set; }
            public string ChoferNombre { get; set; } = "";
            public int Capacidad { get; set; }
            public int Ocupados { get; set; }
            public string Estado { get; set; } = "PROGRAMADO";
            public string CodigoViaje { get; set; } = "";
        }

        // Seatmap del endpoint /Viaje/{id}/seatmap
        public class SeatmapSeatDto
        {
            public int IdAsiento { get; set; }
            public int Numero { get; set; }
            public string EstadoSeat { get; set; } = "LIBRE"; // LIBRE | RESERVADO | OCUPADO | NO_EXISTE

            // Info si no es libre (para tooltip/panel)
            public int? IdBoleto { get; set; }
            public string? ClienteNombre { get; set; }
            public string? ClienteCI { get; set; }
            public decimal? Precio { get; set; }
            public int? OrigenParadaId { get; set; }
            public int? DestinoParadaId { get; set; }
        }

        // Resultado del /Boleto/confirmar
        public class ConfirmarBoletosResultDto
        {
            public List<int> Ok { get; set; } = new();
            public List<ConfirmarFailItem> Fail { get; set; } = new();
            public decimal TotalCobrado { get; set; }
        }
        public class ConfirmarFailItem { public int Id { get; set; } public string Motivo { get; set; } = ""; }

        // Estado por fila expandida
        public class RowUiState
        {
            public ViajePlanillaItem? Viaje { get; set; }

            // Paradas del viaje
            public List<ParadaDto> Paradas { get; set; } = new();
            public int OrigenParadaId { get; set; }
            public int DestinoParadaId { get; set; }

            // Seatmap por tramo
            public List<SeatmapSeatDto> Seatmap { get; set; } = new();

            // Selección local: seatId -> boletoId BLOQUEADO
            public Dictionary<int, int> Seleccionados { get; set; } = new();

            // Cliente / Pago
            public ClienteDto Cliente { get; set; } = new();
            public decimal PrecioUnitario { get; set; } = 0.00M;
            public string MetodoPago { get; set; } = "EFECTIVO"; // EFECTIVO | QR | TRANSFERENCIA
            public string? ReferenciaPago { get; set; }

            public bool Loading { get; set; }
        }

        // ===================== Helpers de layout de asientos =====================
        public class SeatRow
        {
            public bool IsLastRow { get; set; }
            // filas normales (2 – pasillo – 2)
            public SeatmapSeatDto? A1 { get; set; }
            public SeatmapSeatDto? A2 { get; set; }
            public SeatmapSeatDto? A3 { get; set; }
            public SeatmapSeatDto? A4 { get; set; }
            // última fila
            public int LastRowSeats { get; set; } // 4 o 5
            public SeatmapSeatDto? L1 { get; set; }
            public SeatmapSeatDto? L2 { get; set; }
            public SeatmapSeatDto? L3 { get; set; }
            public SeatmapSeatDto? L4 { get; set; }
            public SeatmapSeatDto? L5 { get; set; }
        }

        /// <summary>
        /// Construye filas 2–pasillo–2 y última fila adaptable (41 -> 5; 37 -> 4).
        /// </summary>
        public static List<SeatRow> BuildSeatRows(List<SeatmapSeatDto> seats, int capacidad)
        {
            var list = seats.OrderBy(s => s.Numero).ToList();
            var filas = new List<SeatRow>();

            // 9 filas de 4 asientos (1..36): 2–pasillo–2
            int rowsOfFour = Math.Min(9, (int)Math.Floor(Math.Min(list.Count, capacidad) / 4.0));
            int idx = 0;
            for (int r = 0; r < rowsOfFour && idx + 3 < list.Count; r++)
            {
                filas.Add(new SeatRow
                {
                    A1 = list[idx++],
                    A2 = list[idx++],
                    A3 = list[idx++],
                    A4 = list[idx++],
                    IsLastRow = false
                });
            }

            // Última fila según capacidad
            int restante = list.Count - idx;
            if (capacidad >= 41)
            {
                // 41 ⇒ última fila de 5
                var row = new SeatRow { IsLastRow = true, LastRowSeats = 5 };
                if (restante > 0) row.L1 = list[idx++]; if (restante-- > 1) row.L2 = list[idx++];
                if (restante > 0) row.L3 = list[idx++]; if (restante-- > 1) row.L4 = list[idx++];
                if (restante > 0) row.L5 = list[idx++];
                filas.Add(row);
            }
            else
            {
                // 37 ⇒ última fila de 4
                var row = new SeatRow { IsLastRow = true, LastRowSeats = 4 };
                if (restante > 0) row.L1 = list[idx++]; if (restante-- > 1) row.L2 = list[idx++];
                if (restante > 0) row.L3 = list[idx++]; if (restante-- > 1) row.L4 = list[idx++];
                filas.Add(row);
            }

            return filas;
        }
    }
}
