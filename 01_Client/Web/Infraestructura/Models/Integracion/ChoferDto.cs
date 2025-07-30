using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models.Integracion
{
    public class ChoferDto
    {
        public int IdChofer { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El número de carnet es obligatorio")]
        [Range(1000000, 999999999, ErrorMessage = "Carnet inválido")]
        public int Carnet { get; set; }

        [Required(ErrorMessage = "El número de celular es obligatorio")]
        [Range(60000000, 79999999, ErrorMessage = "Celular inválido")]
        public int Celular { get; set; }

        [Required(ErrorMessage = "La licencia es obligatoria")]
        public string Licencia { get; set; }
    }
}
