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
    [Table("horario", Schema = "public")]
    public partial class Horario : AuditableBaseEntity
    {
        [Key]
        public int IdHorario { get; set; }

        public TimeSpan HoraSalida { get; set; }

        public string DiaSemana { get; set; }

        // Clave foránea a Ruta
        [ForeignKey("Ruta")]
        public int IdRuta { get; set; }
        public Ruta Ruta { get; set; }  // Propiedad navegación
    }
}
