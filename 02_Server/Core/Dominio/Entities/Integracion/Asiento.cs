using Dominio.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Entities.Integracion
{
    [Table("asiento", Schema = "public")]
    public class Asiento : AuditableBaseEntity
    {
        [Key]
        public int IdAsiento { get; set; }

        [ForeignKey("Bus")]
        public int IdBus { get; set; }

        public int Numero { get; set; }

        // Relación de navegación
        public virtual Bus Bus { get; set; }
    }
}
