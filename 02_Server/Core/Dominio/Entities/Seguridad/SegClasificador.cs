using Dominio.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Dominio.Entities.Seguridad
{

    [Table("seg_clasificador", Schema = "public")]
    public partial class SegClasificador : AuditableBaseEntity
    {
        [Key]
        public int IdsegClasificador { get; set; }
        public string NombreClasificador { get; set; }
        public string DescripcionClasificador { get; set; }
        public char? Activo { get; set; }
        public string Valor { get; set; }
        public int? IdsegClasificadortipo { get; set; }
    }
}
