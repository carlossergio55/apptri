using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infraestructura.Abstract;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Linq;
using Infraestructura.Models.Clasificador;
using MudBlazor;
using Infraestructura.Models.Integracion;

namespace Server.Pages.Pages.Integracion
{
    public partial class Comprar
    {
        private int _rutaSeleccionadaId;
        public List<RutaDto> _listaRutas = new List<RutaDto>();
        protected async Task GetRutas()
        {
            try
            {
                _Loading.Show();
                var result = await _Rest.GetAsync<List<RutaDto>>("Ruta/ruta");
                _Loading.Hide();

                if (result.State == State.Success)
                    _listaRutas = result.Data;
                else
                    _MessageShow($"Error: {result.Message}", State.Error);
            }
            catch (Exception ex)
            {
                _Loading.Hide();
                _MessageShow($"Excepción: {ex.Message}", State.Error);
            }
        }
        protected override async Task OnInitializedAsync()
        {
            await GetRutas();
        
        }
    }
}
