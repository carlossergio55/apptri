using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Aplicacion.DTOs.Segurity
{
    public class SegUsuarioDto
    {
        public int IdsegUsuarioSistema { get; set; }
        public string NombreCompleto { get; set; }        
        public string NroCi { get; set; }
        public string Espedido { get; set; }        
        public int IdsegPerfil { get; set; }
        public string Perfil { get; set; }
        public string LoginUsuario { get; set; }        
        public int IdgenInstitucionsucursal { get; set; }
        public int IdgenInstitucion { get; set; }
        public string Institucion { get; set; }
        public string Sucursal { get; set; }        
        public string Estado { get; set; }
       
        public List<string> Roles { get; set; }
        public string JwToken { get; set; }
        [JsonIgnore]
        public string RefreshToken { get; set; }

    }
}
