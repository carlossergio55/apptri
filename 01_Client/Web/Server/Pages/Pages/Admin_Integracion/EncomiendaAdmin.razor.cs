using Dominio.Entities.Integracion;
using Infraestructura.Abstract;
using Infraestructura.Models.Integracion;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Server.Pages.Pages.Admin_Integracion
{
    public partial class EncomiendaAdmin
    {
        private bool expande = false;

        public EncomiendaDto _Encomienda = new EncomiendaDto();
        public List<EncomiendaDto> _encomiendas = new();
        public List<ViajeDto> _viajes = new();

        // --- CRUD -------------------------------------------------------------/Encomienda/encomienda

        private async Task GetEncomiendas()
        {
            var res = await _Rest.GetAsync<List<EncomiendaDto>>("Encomienda/encomienda");
            if (res.State == State.Success)
                _encomiendas = res.Data;
            else
                _MessageShow(res.Message, State.Warning);
        }
        private async Task GetViajes()
        {
            var res = await _Rest.GetAsync<List<ViajeDto>>("Viaje/viaje");
            if (res.State == State.Success)
                _viajes = res.Data;
            else
                _MessageShow(res.Message, State.Warning);
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

        // --- Form handlers ----------------------------------------------------

        private async Task OnValidEncomienda(EditContext _)
        {
            if (_Encomienda.IdEncomienda > 0) await Update(_Encomienda);
            else await Save(_Encomienda);

            _Encomienda = new EncomiendaDto();
            await GetEncomiendas();
            ToggleExpand();
        }

        private void FormEditar(EncomiendaDto dto)
        {
            _Encomienda = dto;
            ToggleExpand();
        }

        private void ResetEncomienda() => _Encomienda = new EncomiendaDto();
        private void ToggleExpand() => expande = !expande;

        // --- Init -------------------------------------------------------------

        protected override async Task OnInitializedAsync() 
        {
            await GetEncomiendas();
            await GetViajes();
        }
    }
}


