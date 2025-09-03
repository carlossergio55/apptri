using FluentValidation;
using Infraestructura.Abstract;
using Infraestructura.Models.Clasificador;
using Infraestructura.Models.Integracion;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Server.Pages.Pages.Integracion
{
    public partial class RastrearEncomienda
    {
        private string numeroGuia;
        private EncomiendaDto? EncomiendaEncontrada;
        private bool MostrandoResultado => EncomiendaEncontrada is not null;

        protected override Task OnInitializedAsync()
        {
            return Task.CompletedTask;
        }

        protected async Task BuscarPorGuia()
        {
            if (string.IsNullOrWhiteSpace(numeroGuia))
            {
                _MessageShow("Debe ingresar un número de guía", State.Warning);
                return;
            }

            try
            {
                _Loading.Show();

                var result = await _Rest.GetAsync<List<EncomiendaDto>>($"Encomienda/GetAllGuia?guiacarga={numeroGuia}");


                _Loading.Hide();

                if (result.State == State.Success && result.Data is not null && result.Data.Count > 0)
                {
                    EncomiendaEncontrada = result.Data.FirstOrDefault();
                }
                else
                {
                    EncomiendaEncontrada = null;
                    _MessageShow($"No se encontró la guía: {numeroGuia}", State.Error);
                }

            }
            catch (Exception ex)
            {
                _Loading.Hide();
                _MessageShow($"Error al buscar la guía: {ex.Message}", State.Error);
            }
        }
 
    }
}
