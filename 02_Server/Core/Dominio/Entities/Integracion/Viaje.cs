using Dominio.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entities.Integracion
{
    [Table("viaje", Schema = "public")]
    public class Viaje : AuditableBaseEntity
    {
        [Key]
        public int IdViaje { get; set; }

        public DateTime Fecha { get; set; }

        public TimeSpan HoraSalida { get; set; }

        public string Estado { get; set; }

        // Relaciones
        public int IdRuta { get; set; }
        [ForeignKey("IdRuta")]
        public virtual Ruta Ruta { get; set; }

        public int IdChofer { get; set; }
        [ForeignKey("IdChofer")]
        public virtual Chofer Chofer { get; set; }

        public int IdBus { get; set; }
        [ForeignKey("IdBus")]
        public virtual Bus Bus { get; set; }
    }
}
