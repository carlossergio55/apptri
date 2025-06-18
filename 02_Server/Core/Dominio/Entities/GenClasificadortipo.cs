using Dominio.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


#nullable disable

namespace Dominio.Entities
{

    [Table("gen_clasificadortipo", Schema = "public")]
    public partial class GenClasificadortipo : AuditableBaseEntity
    {
        [Key]
        public int IdgenClasificadortipo { get; set; }
        public string Descripcion { get; set; }
        public string Valor1 { get; set; }

    }
}
