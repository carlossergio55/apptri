using Dominio.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Dominio.Entities.Seguridad
{
    [Table("seg_sistemar", Schema = "public")]
    public partial class SegSistema : AuditableBaseEntity
    {
        [Key]
        public int IdsegSistema { get; set; }
        public string NombreSistema { get; set; }
        public string DescripcionSistema { get; set; }
        public string UrlSistema { get; set; }
        public string ImagenSistema { get; set; }
        public int? SistemaPosicion { get; set; }
        public string SiglaSistema { get; set; }
        public string VersionCompilacion { get; set; }
        public char? Visible { get; set; }
    }
}
