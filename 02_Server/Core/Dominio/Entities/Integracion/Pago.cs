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

        public string TipoPago { get; set; } // Ej: "Boleto", "Encomienda", etc.

        public int IdReferencia { get; set; } // Puede ser id_boleto o id_encomienda, según el contexto

        public decimal Monto { get; set; }

        public string Metodo { get; set; } // Ej: "Efectivo", "QR", "Tarjeta", etc.

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
