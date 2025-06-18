using Dominio.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Entities.Seguridad
{
    [Table("segv_usuario_epps", Schema = "public")]
    public class SegvUsuario : AuditableBaseEntity
    {
        [Key]
        public int IdsegUsuarioSistema { get; set; }
        public string NroCi { get; set; }
        public string Nombres { get; set; }
        public string ApPaterno { get; set; }
        public string ApMaterno { get; set; }
        public string NombreCompleto { get; set; }
        public int IdcExpedido { get; set; }
        public string Espedido { get; set; }
        public int? IdSistemaAlterno { get; set; }
        public int IdsegPerfil { get; set; }
        public string Perfil { get; set; }
        public string LoginUsuario { get; set; }
        public int IdcTipousuario { get; set; }
        public int IdgenInstitucionsucursal { get; set; }
        public int IdgenInstitucion { get; set; }
        public string Institucion { get; set; }
        public string Sucursal { get; set; }
        public string IdcEstado { get; set; }
        public string Estado { get; set; }
        public string Password { get; set; }
        

    }
}
