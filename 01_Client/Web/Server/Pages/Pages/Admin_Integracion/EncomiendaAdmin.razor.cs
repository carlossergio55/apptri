using Infraestructura.Abstract;
using Infraestructura.Models.Integracion;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server.Pages.Pages.Admin_Integracion
{
    public partial class EncomiendaAdmin
    {
        private bool expande = false;

        public EncomiendaDto _Encomienda = new();
        public List<EncomiendaDto> _encomiendas = new();
        public List<ViajeDto> _viajes = new();
        public List<ParadaDto> _paradas = new();

        // Selecciones de los Autocomplete (nullable)
        private int? _viajeSel;
        private int? _origenSel;
        private int? _destinoSel;

        protected override async Task OnInitializedAsync()
        {
            await GetEncomiendas();
            await GetViajes();
            await GetParadas();
        }

        private async Task GetEncomiendas()
        {
            var res = await _Rest.GetAsync<List<EncomiendaDto>>("Encomienda/encomienda");
            if (res.State == State.Success) _encomiendas = res.Data;
            else _MessageShow(res.Message, State.Warning);
        }

        private async Task GetViajes()
        {
            var res = await _Rest.GetAsync<List<ViajeDto>>("Viaje/viaje");
            if (res.State == State.Success) _viajes = res.Data;
            else _MessageShow(res.Message, State.Warning);
        }

        private async Task GetParadas()
        {
            var res = await _Rest.GetAsync<List<ParadaDto>>("Parada/parada");
            if (res.State == State.Success) _paradas = res.Data;
            else _MessageShow(res.Message, State.Warning);
        }

        private async Task OnValidEncomienda(EditContext _)
        {
            // Pasar las selecciones (int?) al DTO antes de guardar
            _Encomienda.IdViaje = _viajeSel ?? _Encomienda.IdViaje;
            _Encomienda.OrigenParadaId = _origenSel;
            _Encomienda.DestinoParadaId = _destinoSel;

            if (_Encomienda.IdEncomienda > 0)
                await Update(_Encomienda);
            else
                await Save(_Encomienda);

            // Reset
            _Encomienda = new EncomiendaDto();
            _viajeSel = _origenSel = _destinoSel = null;

            await GetEncomiendas();
            ToggleExpand();
        }

        private async Task Save(EncomiendaDto dto)
        {
            _Loading.Show();
            var r = await _Rest.PostAsync<int?>("Encomienda/guardar", new { Encomienda = dto });
            _Loading.Hide();
            _MessageShow(r.Message, r.State);
            if (r.State != State.Success) r.Errors.ForEach(e => _MessageShow(e, State.Warning));
        }

        private async Task Update(EncomiendaDto dto)
        {
            _Loading.Show();
            var r = await _Rest.PutAsync<int>("Encomienda", dto, dto.IdEncomienda);
            _Loading.Hide();
            _MessageShow(r.Message, r.State);
        }

        private void FormEditar(EncomiendaDto dto)
        {
            _Encomienda = new EncomiendaDto
            {
                IdEncomienda = dto.IdEncomienda,
                Remitente = dto.Remitente,
                Destinatario = dto.Destinatario,
                Descripcion = dto.Descripcion,
                Peso = dto.Peso,
                Precio = dto.Precio,
                IdViaje = dto.IdViaje,
                IdGuiaCarga = dto.IdGuiaCarga,
                CodigoGuia = dto.CodigoGuia,
                Estado = dto.Estado,
                Pagado = dto.Pagado,
                OrigenParadaId = dto.OrigenParadaId,
                DestinoParadaId = dto.DestinoParadaId
            };

            // Pre-cargar selecciones en los Autocomplete
            _viajeSel = dto.IdViaje;
            _origenSel = dto.OrigenParadaId;
            _destinoSel = dto.DestinoParadaId;

            ToggleExpand();
        }

        private void ResetEncomienda()
        {
            _Encomienda = new EncomiendaDto();
            _viajeSel = _origenSel = _destinoSel = null;
        }

        private void ToggleExpand() => expande = !expande;

        private async Task Eliminar(int id)
        {
            await _MessageConfirm("¿Eliminar el registro?", async () =>
            {
                var r = await _Rest.DeleteAsync<int>("Encomienda", id);
                if (!r.Succeeded) _MessageShow(r.Message, State.Error);
                else
                {
                    _MessageShow(r.Message, r.State);
                    await GetEncomiendas();
                    StateHasChanged();
                }
            });
        }
    }
}
