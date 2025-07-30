using Infraestructura.Abstract;
using Infraestructura.Models.Integracion;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
namespace Server.Pages.Pages.Admin_Integracion
{
    public partial  class ViajeAdmin
    {
        private bool expande = false;

        public ViajeDto _Viaje = new ViajeDto();

        public List<ViajeDto> _viajes = new();
        public List<ChoferDto> _chofer = new();
        public List<BusDto> _bus = new();


        public string HoraSalida { get; set; }


        public TimeSpan HoraSalidaTime
        {
            get => TimeSpan.TryParse(HoraSalida, out var ts) ? ts : default;
            set => HoraSalida = value.ToString(@"hh\:mm");
        }


        // ----------- CRUD -----------------------------------------------------

        private async Task GetViajes()
        {
            var res = await _Rest.GetAsync<List<ViajeDto>>("Viaje/viaje");
            if (res.State == State.Success)
                _viajes = res.Data;
            else
                _MessageShow(res.Message, State.Warning);
        }
        private async Task GetChofer()
        {
            var res = await _Rest.GetAsync<List<ChoferDto>>("Chofer/chofer");
            if (res.State == State.Success)
                _chofer = res.Data;
            else
                _MessageShow(res.Message, State.Warning);
        }
        private async Task GetBus()
        {
            var res = await _Rest.GetAsync<List<BusDto>>("Bus/bus");
            if (res.State == State.Success)
                _bus = res.Data;
            else
                _MessageShow(res.Message, State.Warning);
        }

        private async Task Save(ViajeDto dto)
        {
            _Loading.Show();
            var r = await _Rest.PostAsync<int?>("Viaje/guardar", new { Viaje = dto });
            _Loading.Hide();
            _MessageShow(r.Message, r.State);
            if (r.State != State.Success) r.Errors.ForEach(e => _MessageShow(e, State.Warning));
        }

        private async Task Update(ViajeDto dto)
        {
            _Loading.Show();
            var r = await _Rest.PutAsync<int>("Viaje", dto, dto.IdViaje);
            _Loading.Hide();
            _MessageShow(r.Message, r.State);
        }

        private async Task Eliminar(int id)
        {
            await _MessageConfirm("¿Eliminar el registro?", async () =>
            {
                var r = await _Rest.DeleteAsync<int>("Viaje", id);
                if (!r.Succeeded) _MessageShow(r.Message, State.Error);
                else
                {
                    _MessageShow(r.Message, r.State);
                    await GetViajes();
                    StateHasChanged();
                }
            });
        }

        // ----------- Form -----------------------------------------------------

        private async Task OnValidViaje(EditContext _)
        {
            if (_Viaje.IdViaje > 0) await Update(_Viaje);
            else await Save(_Viaje);

            _Viaje = new ViajeDto();
            await GetViajes();
            ToggleExpand();
        }

        private void FormEditar(ViajeDto dto)
        {
            _Viaje = dto;
            ToggleExpand();
        }

        private void ResetViaje() => _Viaje = new ViajeDto();
        private void ToggleExpand() => expande = !expande;

        // ----------- Init -----------------------------------------------------

        protected override async Task OnInitializedAsync()
        {
            await GetViajes();
            await GetChofer();
            await GetBus();
        }
       
    }
}
