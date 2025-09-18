using System.ComponentModel.DataAnnotations;

namespace Aplicacion.DTOs.Integracion
{
    public class UsuarioDto
    {
        public int IdUsuario { get; set; }        
        public string Nombre { get; set; }
        public string Correo { get; set; }

        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }    

        public string Tipo { get; set; }
        public int? IdSucursal { get; set; }
    }
}
