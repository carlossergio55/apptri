using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using Infraestructura.Models.Integracion; // Aseg�rate de no tener conflictos
// using Aplicacion.DTOs.Integracion; // Elim�nalo si no lo usas

namespace Server.Pages.Pages.Admin_Integracion
{
    public partial class ChoferAdmin : ComponentBase
    {
        protected ChoferDto nuevoChofer = new();
        protected List<ChoferDto> listaChoferes = new();

        protected void RegistrarChofer()
        {
            nuevoChofer.IdChofer = listaChoferes.Count + 1;
            listaChoferes.Add(nuevoChofer);
            nuevoChofer = new();
        }
    }
}
 

