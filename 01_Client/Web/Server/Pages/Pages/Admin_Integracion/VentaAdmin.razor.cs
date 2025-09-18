using Infraestructura.Abstract;
using Infraestructura.Models.Integracion;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Server.Shared.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;


namespace Server.Pages.Pages.Admin_Integracion
{
    public partial class VentaAdmin : MainBaseComponent
    {
        // ========= Endpoints =========
        private const string EP_VIAJES = "Viaje/viaje";
        private const string EP_RUTAS = "Ruta/ruta";
        private const string EP_CHOFER = "Chofer/chofer";
        private const string EP_BUS = "Bus/bus";

        private const string EP_ASIENTO = "Asiento/asiento";

        private const string EP_BOLETO = "Boleto/boleto";
        private const string EP_B_SAVE = "Boleto/guardar";
        private const string EP_B_DEL = "Boleto";
        private const string EP_P_SAVE = "Pago/guardar";
        private const string EP_CLIENTE = "Cliente/cliente";
        private const string EP_C_SAVE = "Cliente/guardar";
        private const string EP_REPROG = "Boleto/reprogramar";

        // ========= Constantes / helpers =========
        private const int TTL_RESERVA_MIN = 60;
        private static bool Es(string? v, string x) => string.Equals(v, x, StringComparison.OrdinalIgnoreCase);
        private static bool EsPagoConRef(string? m) => m is "QR" or "TRANSFERENCIA";

        // ========= Filtros =========
        private DateTime? _selectedDate = DateTime.Today;
        protected DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                var nv = (value ?? DateTime.Today).Date;
                if (_selectedDate != nv)
                {
                    _selectedDate = nv;
                    _ = CargarPlanilla();
                }
            }
        }
        protected int _diasFiltro = 1;

        // ========= Planilla =========
        protected bool IsLoading { get; set; }
        protected List<ViajePlanillaItem> _planilla = new();

        // ========= Venta / UI =========
        protected bool _ventaDialogVisible, _asientosDialogVisible;

        protected VentaVm? _ventaVm;
        private ViajePlanillaItem? _viajeActual;

        protected AsientoSeatDto? _asientoSeleccionado;
        protected AsientoSeatDto? _seatAction;

        protected ClienteDto _cliente = new();
        protected string? _pagoMetodo = "EFECTIVO";
        protected string? _pagoReferencia;
        protected decimal _precio = 0m;

        protected List<(AsientoSeatDto a1, AsientoSeatDto a2, AsientoSeatDto a3, AsientoSeatDto a4)> _seatRows = new();
        protected (AsientoSeatDto a1, AsientoSeatDto a2, AsientoSeatDto a3, AsientoSeatDto a4, AsientoSeatDto a5)? _lastRow5;

        protected List<int> _missingSeatNumbers = new();

        // Reprogramación
        private AsientoSeatDto? _reprogOrigenSeat;
        private int? _reprogOrigenBoletoId;
        private AsientoSeatDto? _reprogDestinoSeat;
        private bool _puedeConfirmarReprogramacion =>
            _reprogOrigenSeat is not null &&
            _reprogOrigenBoletoId.HasValue &&
            _reprogDestinoSeat is not null &&
            Es(_reprogDestinoSeat!.EstadoSeat, "LIBRE");

        // Venta simple (habilitar botón)
        protected bool _puedeConfirmarVenta =>
            _ventaVm != null &&
            _asientoSeleccionado != null &&
            !string.IsNullOrWhiteSpace(_cliente?.Nombre) &&
            _precio > 0 &&
            !string.IsNullOrWhiteSpace(_pagoMetodo) &&
            (!EsPagoConRef(_pagoMetodo) || !string.IsNullOrWhiteSpace(_pagoReferencia));

        // ========= Ciclo de vida =========
        protected override async Task OnInitializedAsync() => await CargarPlanilla();
        protected async Task SetHoy() { SelectedDate = DateTime.Today; await CargarPlanilla(); }
        protected async Task SetManana() { SelectedDate = DateTime.Today.AddDays(1); await CargarPlanilla(); }

        // ========= Helpers REST compactos =========
        private async Task<List<T>> GetList<T>(string ep)
        {
            var r = await _Rest.GetAsync<List<T>>(ep);
            return r.State == State.Success && r.Data != null ? r.Data : new();
        }
        private async Task<int?> PostId(string ep, object body)
        {
            var r = await _Rest.PostAsync<int?>(ep, body);
            return r.State == State.Success ? r.Data : null;
        }
        private async Task<bool> DeleteId(string ep, int id)
        {
            var r = await _Rest.DeleteAsync<int>(ep, id);
            return r.Succeeded;
        }

        // ========= PLANILLA =========
        protected async Task CargarPlanilla()
        {
            IsLoading = true;
            _planilla.Clear();
            try
            {
                var fecha = (SelectedDate ?? DateTime.Today).Date;
                var desde = fecha;
                var hasta = fecha.AddDays(Math.Max(1, _diasFiltro) - 1);

                var viajes = await GetList<ViajeDto>(EP_VIAJES);
                var rutas = await GetList<RutaDto>(EP_RUTAS);
                var chof = await GetList<ChoferDto>(EP_CHOFER);
                var buses = await GetList<BusDto>(EP_BUS);

                var rango = viajes
                    .Where(v => v.Fecha.Date >= desde && v.Fecha.Date <= hasta)
                    .OrderBy(v => v.Fecha).ThenBy(v => v.HoraSalida);

                _planilla = rango.Select(v =>
                {
                    var ruta = rutas.FirstOrDefault(x => x.IdRuta == v.IdRuta);
                    var cho = chof.FirstOrDefault(x => x.IdChofer == v.IdChofer);
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
                        ChoferNombre = cho?.Nombre ?? "-",
                        Capacidad = bus?.Capacidad ?? 0,
                        Estado = v.Estado ?? "PROGRAMADO",
                        CodigoViaje = v.IdViaje.ToString()
                    };
                }).ToList();
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

        protected Color EstadoColor(string? e) => (e ?? "PROGRAMADO").ToUpperInvariant() switch
        {
            "PROGRAMADO" => Color.Info,
            "EMBARCANDO" => Color.Warning,
            "ENRUTA" => Color.Secondary,
            "FINALIZADO" => Color.Success,
            "CANCELADO" => Color.Error,
            _ => Color.Default
        };
        protected bool PuedeVender(string? e) => (e ?? "PROGRAMADO").ToUpperInvariant() is "PROGRAMADO" or "EMBARCANDO";

        private async Task RefrescarFila(ViajePlanillaItem v)
        {
            await CargarAsientos(v);
            _ventaVm = null;
            _asientoSeleccionado = _seatAction = null;
        }
        private async Task RefrescarDialog()
        {
            if (_viajeActual != null) await CargarAsientos(_viajeActual);
        }

        // ========= VENTA / ASIENTOS =========
        protected async Task AbrirVenta(ViajePlanillaItem v)
        {
            _viajeActual = v;
            await CargarAsientos(v);
            _cliente = new();
            _pagoMetodo = "EFECTIVO";
            _pagoReferencia = null;
            _precio = 0;
            _reprogOrigenSeat = null; _reprogOrigenBoletoId = null; _reprogDestinoSeat = null;
            _ventaDialogVisible = true;
        }
        protected async Task VerAsientos(ViajePlanillaItem v)
        {
            _viajeActual = v;
            await CargarAsientos(v);
            _asientosDialogVisible = true;
        }

        private async Task CargarAsientos(ViajePlanillaItem v)
        {
            try
            {
                _asientoSeleccionado = _seatAction = null;

                // 1) plantilla por BUS
                var plantilla = (await GetList<AsientoDto>(EP_ASIENTO))
                    .Where(a => a.IdBus == v.IdBus).ToList();

                int capacidad = v.Capacidad;
                var porNumero = plantilla
                    .Where(p => p.Numero is >= 1 and <= int.MaxValue)
                    .ToDictionary(p => p.Numero);

                _missingSeatNumbers.Clear();
                var completos = new List<AsientoDto>(capacidad);
                for (int n = 1; n <= capacidad; n++)
                    completos.Add(porNumero.TryGetValue(n, out var p) ? p : Missing(n));

                AsientoDto Missing(int n)
                {
                    _missingSeatNumbers.Add(n);
                    return new AsientoDto { IdAsiento = 0, IdBus = v.IdBus, Numero = n };
                }

                // 2) boletos del VIAJE
                var boletos = (await GetList<BoletoDto>(EP_BOLETO))
                              .Where(b => b.IdViaje == v.IdViaje).ToList();

                var ahora = DateTime.Now;
                var seatMap = completos.Select(p =>
                {
                    var existe = p.IdAsiento > 0;
                    var bs = existe
                        ? boletos.Where(b => b.IdAsiento == p.IdAsiento)
                                 .OrderByDescending(b => b.IdBoleto).ToList()
                        : new List<BoletoDto>();

                    var pagado = bs.FirstOrDefault(b => (b.Estado ?? "").ToUpper() is "PAGADO" or "EMBARCADO");
                    var reservado = bs.FirstOrDefault(b => (b.Estado ?? "").ToUpper() == "BLOQUEADO"
                                                   && (ahora - b.FechaCompra).TotalMinutes <= TTL_RESERVA_MIN);

                    string estado = !existe ? "NO_EXISTE" :
                                    pagado != null ? "OCUPADO" :
                                    reservado != null ? "RESERVADO" : "LIBRE";

                    var activo = pagado ?? reservado;

                    return new AsientoSeatDto
                    {
                        IdAsiento = p.IdAsiento,
                        Numero = p.Numero,
                        EstadoSeat = estado,
                        IdBoleto = activo?.IdBoleto,
                        EstadoBoleto = activo?.Estado,
                        PrecioBoleto = activo?.Precio,
                        IdClienteBoleto = activo?.IdCliente
                    };
                })
                .OrderBy(s => s.Numero)
                .ToList();

                var (rows4, row5) = BuildSeatLayout(seatMap, capacidad);
                _seatRows = rows4; _lastRow5 = row5;

                var item = _planilla.FirstOrDefault(x => x.IdViaje == v.IdViaje);
                if (item != null)
                    item.Ocupados = seatMap.Count(s => s.EstadoSeat is "OCUPADO" or "RESERVADO");

                _ventaVm = new VentaVm
                {
                    IdViaje = v.IdViaje,
                    RutaNombre = v.RutaNombre,
                    HoraSalida = v.HoraSalida,
                    Placa = v.Placa,
                    Asientos = seatMap
                };
            }
            catch (Exception ex)
            {
                _MessageShow($"Error al cargar asientos: {ex.Message}", State.Error);
            }
        }

        private static (
            List<(AsientoSeatDto, AsientoSeatDto, AsientoSeatDto, AsientoSeatDto)> rows4,
            (AsientoSeatDto, AsientoSeatDto, AsientoSeatDto, AsientoSeatDto, AsientoSeatDto)? last5
        ) BuildSeatLayout(List<AsientoSeatDto> seats, int capacidad)
        {
            var byNum = seats.ToDictionary(s => s.Numero);
            AsientoSeatDto Get(int x) => byNum.TryGetValue(x, out var s)
                ? s : new AsientoSeatDto { IdAsiento = 0, Numero = x, EstadoSeat = "NO_EXISTE" };

            bool last5 = capacidad % 4 == 1;
            int full = last5 ? (capacidad - 5) / 4 : capacidad / 4;

            var rows4 = new List<(AsientoSeatDto, AsientoSeatDto, AsientoSeatDto, AsientoSeatDto)>();
            (AsientoSeatDto, AsientoSeatDto, AsientoSeatDto, AsientoSeatDto, AsientoSeatDto)? last = null;

            int n = 1;
            for (int i = 0; i < full; i++) rows4.Add((Get(n++), Get(n++), Get(n++), Get(n++)));
            if (last5) last = (Get(n++), Get(n++), Get(n++), Get(n++), Get(n++));

            return (rows4, last);
        }

        // ========= Render helpers =========
        private static (Color color, Variant variant) EstiloSeat(bool seleccionado, bool ocupado, bool reservado) =>
            (seleccionado ? Color.Primary
             : ocupado ? Color.Error
             : reservado ? Color.Warning : Color.Default,
             (seleccionado || ocupado || reservado) ? Variant.Filled : Variant.Outlined);

        private RenderFragment SeatButton(AsientoSeatDto a) => builder =>
        {
            if (a is null) return;

            bool noExiste = a.IdAsiento <= 0 || string.Equals(a.EstadoSeat, "NO_EXISTE", StringComparison.OrdinalIgnoreCase);
            bool esReservado = string.Equals(a.EstadoSeat, "RESERVADO", StringComparison.OrdinalIgnoreCase);
            bool esOcupado = string.Equals(a.EstadoSeat, "OCUPADO", StringComparison.OrdinalIgnoreCase);
            bool seleccionado = (_asientoSeleccionado?.Numero == a.Numero) || (_seatAction?.Numero == a.Numero);

            var color =
                seleccionado ? Color.Primary :
                esOcupado ? Color.Error :
                esReservado ? Color.Warning :
                               Color.Default;

            var variant = (seleccionado || esOcupado || esReservado) ? Variant.Filled : Variant.Outlined;

            builder.OpenComponent<MudButton>(0);
            builder.AddAttribute(1, "Class", "seat-btn");
            builder.AddAttribute(2, "Color", color);
            builder.AddAttribute(3, "Variant", variant);
            builder.AddAttribute(4, "Disabled", noExiste);

            // 👇 cambio clave: EventCallback<MouseEventArgs>
            builder.AddAttribute(5, "OnClick",
                EventCallback.Factory.Create<MouseEventArgs>(this, _ => SeleccionarAsiento(a)));

            builder.AddAttribute(6, "ChildContent", (RenderFragment)(b => b.AddContent(7, a.Numero)));
            builder.CloseComponent();
        };


        private RenderFragment StaticSeat(AsientoSeatDto a) => builder =>
        {
            if (a is null) return;
            var (color, variant) = EstiloSeat(false, Es(a.EstadoSeat, "OCUPADO"), Es(a.EstadoSeat, "RESERVADO"));

            builder.OpenComponent<MudButton>(0);
            builder.AddAttribute(1, "Class", "seat-btn");
            builder.AddAttribute(2, "Color", color);
            builder.AddAttribute(3, "Variant", variant);
            builder.AddAttribute(4, "Disabled", true);
            builder.AddAttribute(5, "ChildContent", (RenderFragment)(b => b.AddContent(6, a.Numero)));
            builder.CloseComponent();
        };

        // ========= Interacción =========
        protected async Task SeleccionarAsiento(AsientoSeatDto a)
        {
            _seatAction = a;

            if (Es(a.EstadoSeat, "LIBRE"))
                _asientoSeleccionado = a;
            else
            {
                _asientoSeleccionado = null;
                if (a.PrecioBoleto.HasValue) _precio = a.PrecioBoleto.Value;
            }
            StateHasChanged();
        }

        // ========= Cliente =========
        private async Task<int> EnsureClienteId()
        {
            if (_cliente.IdCliente > 0) return _cliente.IdCliente;

            var lista = await GetList<ClienteDto>(EP_CLIENTE);
            var existente = lista.FirstOrDefault(c =>
                (c.Carnet == _cliente.Carnet && c.Carnet != 0) ||
                (!string.IsNullOrWhiteSpace(c.Nombre) &&
                 c.Nombre.Trim().Equals(_cliente.Nombre?.Trim(), StringComparison.OrdinalIgnoreCase) &&
                 c.Celular == _cliente.Celular));

            if (existente != null) return existente.IdCliente;

            var nuevo = new ClienteDto
            {
                Nombre = _cliente.Nombre?.Trim() ?? "",
                Carnet = _cliente.Carnet,
                Celular = _cliente.Celular,
                Correo = _cliente.Correo
            };

            var id = await PostId(EP_C_SAVE, new { Cliente = nuevo });
            if (id is null or <= 0) throw new InvalidOperationException("No se pudo registrar el cliente.");
            return id.Value;
        }

        // ========= Selección múltiple =========
        private readonly List<AsientoSeatDto> _seleccionados = new();
        private decimal _total => (_precio > 0 ? _precio : 0) * _seleccionados.Count;

        private bool _puedeConfirmarVentaMulti =>
            _ventaVm != null && _seleccionados.Count > 0 &&
            !string.IsNullOrWhiteSpace(_cliente?.Nombre) &&
            _precio > 0 && !string.IsNullOrWhiteSpace(_pagoMetodo) &&
            (!EsPagoConRef(_pagoMetodo) || !string.IsNullOrWhiteSpace(_pagoReferencia));

        private void LimpiarSeleccion()
        {
            _seleccionados.Clear();
            _asientoSeleccionado = _seatAction = null;
            StateHasChanged();
        }
        private void ToggleSeat(AsientoSeatDto a)
        {
            if (a is null || !Es(a.EstadoSeat, "LIBRE") || a.IdAsiento <= 0) return;
            var ya = _seleccionados.Any(s => s.IdAsiento == a.IdAsiento);
            if (ya) _seleccionados.RemoveAll(s => s.IdAsiento == a.IdAsiento);
            else _seleccionados.Add(a);
            StateHasChanged();
        }

        // ========= núcleo: emitir boleto + pago =========
        private async Task<(bool ok, int boletoId, string? msg)> EmitirBoletoYPago(
            int idViaje, int idAsiento, int idCliente, decimal precio, string metodo, string? referencia)
        {
            var boleto = new BoletoDto
            {
                IdViaje = idViaje,
                IdAsiento = idAsiento,
                IdCliente = idCliente,
                Precio = precio,
                Estado = "PAGADO",
                FechaCompra = DateTime.Now
            };

            var idB = await PostId(EP_B_SAVE, new { Boleto = boleto });
            if (idB is null or <= 0) return (false, 0, "No se pudo guardar el boleto.");

            if (EsPagoConRef(metodo) && string.IsNullOrWhiteSpace(referencia))
            {
                await DeleteId(EP_B_DEL, idB.Value);
                return (false, 0, "La referencia es obligatoria para QR/TRANSFERENCIA.");
            }

            var pago = new PagoDto
            {
                TipoPago = "Boleto",
                IdReferencia = idB.Value,
                Monto = precio,
                Metodo = metodo,
                FechaPago = DateTime.Now,
                IdUsuario = 1,
                IdCliente = idCliente,
                // Referencia = referencia  // si tu DTO ya la expone
            };

            var idP = await PostId(EP_P_SAVE, new { Pago = pago });
            if (idP is null)
            {
                await DeleteId(EP_B_DEL, idB.Value);
                return (false, 0, "Falló el registro del pago.");
            }

            return (true, idB.Value, null);
        }

        // ========= Acciones: Reservar / Pagar / Anular =========
        public async Task ReservarAsiento()
        {
            if (_ventaVm is null || _seatAction is null || _seatAction.IdAsiento <= 0 || !Es(_seatAction.EstadoSeat, "LIBRE"))
            { _MessageShow("Selecciona un asiento LIBRE.", State.Warning); return; }
            if (string.IsNullOrWhiteSpace(_cliente?.Nombre))
            { _MessageShow("Registra el cliente (Nombre).", State.Warning); return; }
            if (_precio <= 0) { _MessageShow("Indica el precio antes de reservar.", State.Warning); return; }

            try
            {
                _Loading.Show();
                int idCliente = await EnsureClienteId();

                var id = await PostId(EP_B_SAVE, new
                {
                    Boleto = new BoletoDto
                    {
                        IdViaje = _ventaVm.IdViaje,
                        IdAsiento = _seatAction.IdAsiento,
                        IdCliente = idCliente,
                        Precio = _precio,
                        Estado = "BLOQUEADO",
                        FechaCompra = DateTime.Now
                    }
                });

                if (id is null or <= 0) _MessageShow("No se pudo reservar el asiento.", State.Warning);
                else
                {
                    _MessageShow($"Asiento {_seatAction.Numero} reservado.", State.Success);
                    await RefrescarDialog();
                }
            }
            catch (Exception ex) { _MessageShow($"No se pudo reservar: {ex.Message}", State.Error); }
            finally { _Loading.Hide(); }
        }

        public async Task PagarReserva()
        {
            if (_ventaVm is null || _seatAction is null || !Es(_seatAction.EstadoSeat, "RESERVADO") || !_seatAction.IdBoleto.HasValue)
            { _MessageShow("Selecciona un asiento RESERVADO.", State.Warning); return; }

            try
            {
                _Loading.Show();

                var boletoUpd = new BoletoDto
                {
                    IdBoleto = _seatAction.IdBoleto!.Value,
                    IdViaje = _ventaVm.IdViaje,
                    IdAsiento = _seatAction.IdAsiento,
                    IdCliente = _seatAction.IdClienteBoleto ?? 0,
                    Precio = _seatAction.PrecioBoleto ?? _precio,
                    Estado = "PAGADO",
                    FechaCompra = DateTime.Now
                };

                var id = await PostId(EP_B_SAVE, new { Boleto = boletoUpd });
                if (id is null)
                { _MessageShow("No se pudo confirmar el boleto.", State.Warning); return; }

                if (EsPagoConRef(_pagoMetodo) && string.IsNullOrWhiteSpace(_pagoReferencia))
                { _MessageShow("La referencia es obligatoria para QR/TRANSFERENCIA.", State.Warning); return; }

                var pagoId = await PostId(EP_P_SAVE, new
                {
                    Pago = new PagoDto
                    {
                        TipoPago = "Boleto",
                        IdReferencia = boletoUpd.IdBoleto,
                        Monto = boletoUpd.Precio,
                        Metodo = _pagoMetodo!,
                        FechaPago = DateTime.Now,
                        IdUsuario = 1,
                        IdCliente = boletoUpd.IdCliente
                    }
                });

                if (pagoId is null)
                {
                    // rollback a reserva
                    await PostId(EP_B_SAVE, new
                    {
                        Boleto = new BoletoDto
                        {
                            IdBoleto = boletoUpd.IdBoleto,
                            IdViaje = boletoUpd.IdViaje,
                            IdAsiento = boletoUpd.IdAsiento,
                            IdCliente = boletoUpd.IdCliente,
                            Precio = boletoUpd.Precio,
                            Estado = "BLOQUEADO",
                            FechaCompra = DateTime.Now
                        }
                    });
                    _MessageShow("Falló el pago de la reserva.", State.Warning);
                    return;
                }

                _MessageShow("Reserva pagada.", State.Success);
                await RefrescarDialog();
            }
            catch (Exception ex) { _MessageShow($"No se pudo pagar la reserva: {ex.Message}", State.Error); }
            finally { _Loading.Hide(); }
        }

        public async Task AnularReserva()
        {
            if (_seatAction?.EstadoSeat != "RESERVADO" || !_seatAction.IdBoleto.HasValue)
            { _MessageShow("Selecciona un asiento RESERVADO.", State.Warning); return; }

            await _MessageConfirm($"¿Anular la reserva del asiento {_seatAction.Numero}?", async () =>
            {
                if (!await DeleteId(EP_B_DEL, _seatAction.IdBoleto.Value))
                    _MessageShow("No se pudo anular la reserva.", State.Error);
                else
                {
                    _MessageShow("Reserva anulada.", State.Success);
                    await RefrescarDialog();
                }
            });
        }

        public async Task AnularBoletoPagado()
        {
            _MessageShow("Anulación de boleto pagado no permitida por política. Usa reprogramación.", State.Success);
            await Task.CompletedTask;
        }

        // ========= Reprogramación =========
        public void IniciarReprogramacion()
        {
            if (_seatAction is null || !_seatAction.IdBoleto.HasValue)
            { _MessageShow("Selecciona primero un asiento con boleto (reservado/ocupado).", State.Warning); return; }

            _reprogOrigenSeat = _seatAction;
            _reprogOrigenBoletoId = _seatAction.IdBoleto;
            _reprogDestinoSeat = null;
            _MessageShow($"Origen: asiento {_reprogOrigenSeat.Numero}. Elige ahora un asiento LIBRE destino.", State.Success);
        }

        public void SeleccionarDestinoReprogramacion(AsientoSeatDto a)
        {
            if (_reprogOrigenSeat is null || !_reprogOrigenBoletoId.HasValue)
            { _MessageShow("Primero elige el asiento de origen (con boleto) para reprogramar.", State.Warning); return; }

            if (!Es(a.EstadoSeat, "LIBRE") || a.IdAsiento <= 0)
            { _MessageShow("El destino debe ser un asiento LIBRE.", State.Warning); return; }

            _reprogDestinoSeat = a;
            _MessageShow($"Destino tentativo: asiento {a.Numero}.", State.Success);
        }

        public async Task ConfirmarReprogramacion(string motivo = "Reprogramación operativa")
        {
            if (!_puedeConfirmarReprogramacion || _ventaVm is null)
            { _MessageShow("Selecciona origen (con boleto) y destino (LIBRE) para reprogramar.", State.Warning); return; }

            try
            {
                _Loading.Show();
                var id = await PostId(EP_REPROG, new
                {
                    IdBoleto = _reprogOrigenBoletoId!.Value,
                    IdViajeDestino = _ventaVm.IdViaje,
                    IdAsientoDestino = _reprogDestinoSeat!.IdAsiento,
                    IdUsuario = 1,
                    Motivo = motivo
                });

                if (id is null) { _MessageShow("No se pudo reprogramar el boleto.", State.Warning); return; }

                _MessageShow("Boleto reprogramado.", State.Success);
                _reprogOrigenSeat = null; _reprogOrigenBoletoId = null; _reprogDestinoSeat = null;
                await RefrescarDialog();
            }
            catch (Exception ex) { _MessageShow($"Error al reprogramar: {ex.Message}", State.Error); }
            finally { _Loading.Hide(); }
        }
        public void CancelarReprogramacion()
        {
            _reprogOrigenSeat = null; _reprogOrigenBoletoId = null; _reprogDestinoSeat = null;
            _MessageShow("Reprogramación cancelada.", State.Success);
        }

        // ========= Venta directa =========
        protected async Task ConfirmarVenta()
        {
            if (_ventaVm is null || _asientoSeleccionado is null)
            { _MessageShow("Selecciona un asiento y completa los datos.", State.Warning); return; }
            if (_asientoSeleccionado.IdAsiento <= 0)
            { _MessageShow($"El asiento {_asientoSeleccionado.Numero} no existe en el backend.", State.Warning); return; }

            try
            {
                _Loading.Show();
                int idCliente = await EnsureClienteId();

                var (ok, boletoId, msg) = await EmitirBoletoYPago(
                    _ventaVm.IdViaje, _asientoSeleccionado.IdAsiento, idCliente, _precio, _pagoMetodo!, _pagoReferencia);

                if (!ok) { _MessageShow(msg ?? "No se pudo emitir el boleto.", State.Warning); return; }

                _MessageShow($"Boleto #{boletoId} emitido. Total Bs {_precio:0.00}", State.Success);
                _ventaDialogVisible = false;
                await CargarPlanilla();
            }
            catch (Exception ex) { _MessageShow($"No se pudo emitir el boleto: {ex.Message}", State.Error); }
            finally { _Loading.Hide(); }
        }

        // ========= Venta múltiple =========
        private async Task ConfirmarVentaMultiple()
        {
            if (!_puedeConfirmarVentaMulti || _ventaVm is null) return;

            try
            {
                _Loading.Show();
                int idCliente = await EnsureClienteId();

                int ok = 0, fail = 0;
                foreach (var s in _seleccionados.ToList())
                {
                    if (s is null || s.IdAsiento <= 0 || !Es(s.EstadoSeat, "LIBRE")) { fail++; continue; }

                    var (bien, boletoId, msg) = await EmitirBoletoYPago(
                        _ventaVm.IdViaje, s.IdAsiento, idCliente, _precio, _pagoMetodo!, _pagoReferencia);

                    if (bien) { ok++; _seleccionados.RemoveAll(x => x.IdAsiento == s.IdAsiento); }
                    else { fail++; }
                }

                _MessageShow(ok > 0 && fail == 0
                    ? $"Se emitieron {ok} boletos correctamente."
                    : ok > 0 ? $"Parcial: {ok} emitidos, {fail} con error."
                             : "No se pudo emitir ningún boleto.", ok > 0 ? State.Success : State.Warning);

                await CargarPlanilla();
                await RefrescarDialog();
            }
            catch (Exception ex) { _MessageShow($"Error en venta múltiple: {ex.Message}", State.Error); }
            finally { _Loading.Hide(); StateHasChanged(); }
        }

        protected void CerrarDialogo()
        {
            _ventaDialogVisible = _asientosDialogVisible = false;
            _ventaVm = null;
            _asientoSeleccionado = _seatAction = null;
            _reprogOrigenSeat = null; _reprogOrigenBoletoId = null; _reprogDestinoSeat = null;
            _precio = 0;
        }

        // ========= View Models =========
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

        public class AsientoSeatDto
        {
            public int IdAsiento { get; set; }
            public int Numero { get; set; }
            public string EstadoSeat { get; set; } = "LIBRE";
            public DateTime? HoldExpira { get; set; }
            public int? IdBoleto { get; set; }
            public string? EstadoBoleto { get; set; }
            public decimal? PrecioBoleto { get; set; }
            public int? IdClienteBoleto { get; set; }
        }

        public class VentaVm
        {
            public int IdViaje { get; set; }
            public string RutaNombre { get; set; } = "";
            public string HoraSalida { get; set; } = "";
            public string Placa { get; set; } = "";
            public List<AsientoSeatDto> Asientos { get; set; } = new();
        }
    }
}
