using Dominio.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Dominio.Entities.Seguridad
{
    public partial class SegUsuario: AuditableBaseEntity
    {
        [Key]
        public int IdsegUsuarioSistema { get; set; }

        public string IdSistemaAlterno { get; set; }
        public int? IdsegPerfil { get; set; }

        public string LoginUsuario { get; set; }
        public string Password { get; set; }
        public string NroCi { get; set; }

        public int? IdcTipousuario { get; set; }
        public int IdgenInstitucionsucursal { get; set; }
        public char? IdcEstado { get; set; }

        public virtual SegPerfil IdsegPerfilNavigation { get; set; }
        public virtual SegPersonaExt NroCiNavigation { get; set; }
    }
}
