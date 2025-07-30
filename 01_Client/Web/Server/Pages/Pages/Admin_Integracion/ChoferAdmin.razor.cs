using Dominio.Entities.Integracion;
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
    public partial class ChoferAdmin 
    {
        private void ToggleExpand() => expande = !expande;
        private bool expande = false;
        public ChoferDto _Chofer = new ChoferDto();
        public List<ChoferDto> _chofer = new();
        private async Task GetChofer()
        {
            var res = await _Rest.GetAsync<List<ChoferDto>>("Chofer/chofer"); 
            if (res.State == State.Success)
                _chofer = res.Data;
            else
                _MessageShow(res.Message, State.Warning);
        }
        private async Task SaveChofer(ChoferDto dto)
        {
            _Loading.Show();
            var r = await _Rest.PostAsync<int?>("Chofer/Guardar", new { Chofer = dto });
            _Loading.Hide();
            _MessageShow(r.Message, r.State);
            if (r.State != State.Success) r.Errors.ForEach(e => _MessageShow(e, State.Warning));
        }
        private async Task UpdateChofer(ChoferDto dto)
        {
            _Loading.Show();
            var r = await _Rest.PutAsync<int>("Chofer", dto, dto.IdChofer);
            _Loading.Hide();
            _MessageShow(r.Message, r.State);
        }
        private async Task OnValidChofer(EditContext _)
        {
            if (_Chofer.IdChofer > 0) await UpdateChofer(_Chofer);
            else await SaveChofer(_Chofer);

            _Chofer = new ChoferDto();
            await GetChofer();
            ToggleExpand();
        }
        protected override async Task OnInitializedAsync()
        {
            await GetChofer();
        }
        private void ResetChofer() => _Chofer = new ChoferDto();

    }
}
 

