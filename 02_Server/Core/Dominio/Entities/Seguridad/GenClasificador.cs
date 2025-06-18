using Dominio.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Entities.Seguridad
{

    [Table("gen_clasificador", Schema ="public")]
    public class GenClasificador : AuditableBaseEntity 
    {
        [Key]
        public int IdgenClasificador { get; set; }
        public int IdgenClasificadortipo { get; set; }
        public string Descripcion { get; set; }
        public string Valor2 { get; set; }
        public string Valor3 { get; set; }
        public int? Valor4 { get; set; }
        public int? Valor5 { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

    }
}
