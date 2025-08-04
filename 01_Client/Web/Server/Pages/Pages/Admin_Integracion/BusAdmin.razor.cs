using Infraestructura.Abstract;
using Infraestructura.Models.Integracion;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server.Pages.Pages.Admin_Integracion
{
    public partial class BusAdmin
    {
        private bool expande = false;

        public BusDto _Bus = new();
        public List<BusDto> _buses = new();

        // ---------- CRUD ----------
        private async Task GetBuses()
        {
            var res = await _Rest.GetAsync<List<BusDto>>("Bus/bus");
            if (res.State == State.Success)
                _buses = res.Data;
            else
                _MessageShow(res.Message, State.Warning);
        }

        private async Task Save(BusDto dto)
        {
            _Loading.Show();
            var r = await _Rest.PostAsync<int?>("Bus/guardar", new { Bus = dto });
            _Loading.Hide();
            _MessageShow(r.Message, r.State);
            if (r.State != State.Success) r.Errors.ForEach(e => _MessageShow(e, State.Warning));
        }

        private async Task Update(BusDto dto)
        {
            _Loading.Show();
            var r = await _Rest.PutAsync<int>("Bus", dto, dto.IdBus);
            _Loading.Hide();
            _MessageShow(r.Message, r.State);
        }

        private async Task Eliminar(int id)
        {
            await _MessageConfirm("¿Eliminar el registro?", async () =>
            {
                var r = await _Rest.DeleteAsync<int>("Bus", id);
                if (!r.Succeeded) _MessageShow(r.Message, State.Error);
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

        // ---------- Init ----------
        protected override async Task OnInitializedAsync()
        {
            await GetBuses();
        }
    }
}
