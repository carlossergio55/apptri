using Dominio.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Dominio.Entities.Seguridad
{

    [Table("seg_persona_ext", Schema = "public")]
    public partial class SegPersonaExt : AuditableBaseEntity
    {
        public SegPersonaExt()
        {
            SegUsuario = new HashSet<SegUsuario>();
        }

        [Key]
        public int IdsegPersonaExt { get; set; }
        public string Nombres { get; set; }
        public string ApPaterno { get; set; }
        public string ApMaterno { get; set; }
        public string NroCi { get; set; }
        public int? IdcExpedido { get; set; }
        public int? UnidadAutoriza { get; set; }
        public string EntExterna { get; set; }

        public virtual ICollection<SegUsuario> SegUsuario { get; set; }
    }
}
