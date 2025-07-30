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
    public partial class BusAdmin
    {
        public List<BusDto> _bus = new();
        private async Task GetBus()
        {
            var res = await _Rest.GetAsync<List<BusDto>>("Bus/bus");
            if (res.State == State.Success)
                _bus = res.Data;
            else
                _MessageShow(res.Message, State.Warning);
        }
        protected override async Task OnInitializedAsync()
        {
            await GetBus();
        }
    }
}
