using Dominio.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Dominio.Entities.Seguridad
{

    [Table("seg_perfil", Schema = "public")]
    public partial class SegPerfil : AuditableBaseEntity
    {
        public SegPerfil()
        {
            SegUsuario = new HashSet<SegUsuario>();
        }

        [Key]
        public int IdsegPerfil { get; set; }
        public string NombrePerfil { get; set; }
        public string DescripcionPerfil { get; set; }
       
        public virtual ICollection<SegUsuario> SegUsuario { get; set; }
    }
}
