using Dominio.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Dominio.Entities
{
    [Table("gen_clasificador", Schema = "public")]
    public partial class GenClasificador : AuditableBaseEntity
    {
        [Key]
        public int IdgenClasificador { get; set; }
        public int IdgenClasificadortipo { get; set; }
        public string Descripcion { get; set; }
        public string? Valor2 { get; set; }
        public string? Valor3 { get; set; }
        public int? Valor4 { get; set; }
        public int? Valor5 { get; set; }
 
    }
}
