using System.ComponentModel.DataAnnotations;

namespace Infraestructura.Models.Integracion
{
    public class GuiaCargaDto
    {
        public int IdGuiaCarga { get; set; }

        [Required]
        public string Codigo { get; set; } = string.Empty; // ej: "SAM-0123"
    }
}
