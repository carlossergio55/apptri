using System.Collections.Generic;

namespace Aplicacion.Interfaces
{
    public interface ICurrentUserService
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
    }
}
