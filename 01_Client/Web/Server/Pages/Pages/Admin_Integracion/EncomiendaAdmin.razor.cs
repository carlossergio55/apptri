using Infraestructura.Models.Integracion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Server.Pages.Pages.Admin_Integracion
{
    public class EncomiendaAdminModel : PageModel
    {
        [BindProperty]
        public EncomiendaDto Encomienda { get; set; } = new();

        public List<EncomiendaDto> ListaEncomiendas { get; set; } = new();

        public void OnGet()
        {
            // Aquí puedes cargar datos iniciales si deseas
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // En un escenario real aquí guardarías en base de datos
            ListaEncomiendas.Add(Encomienda);

            // Limpiar el formulario
            Encomienda = new();

            return Page();
        }
    }
}
