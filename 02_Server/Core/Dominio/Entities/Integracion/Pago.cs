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
    [Table("pago", Schema = "public")]
    public class Pago : AuditableBaseEntity
    {
        [Key]
        public int IdPago { get; set; }

        public string TipoPago { get; set; } 

        public int IdReferencia { get; set; } 

        public decimal Monto { get; set; }

        public string Metodo { get; set; } 

        public DateTime FechaPago { get; set; }

        // Relaciones foráneas
        public int IdUsuario { get; set; }
        [ForeignKey("IdUsuario")]
        public virtual Usuario Usuario { get; set; }

        public int IdCliente { get; set; }
        [ForeignKey("IdCliente")]
        public virtual Cliente Cliente { get; set; }
    }
}
