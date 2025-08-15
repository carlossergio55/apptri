using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dominio.Common;
using System;

namespace Dominio.Entities.Integracion
{
    [Table("parada", Schema = "public")]
    public class Parada : AuditableBaseEntity
    {
        [Key]
        [Column("id_parada")]
        public int IdParada { get; set; }

        [Required]
        [MaxLength(60)]
        [Column("nombre")]
        public string Nombre { get; set; } = null!;
    }
}
