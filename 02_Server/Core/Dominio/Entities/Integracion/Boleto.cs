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
    [Table("boleto", Schema = "public")]
    public class Boleto : AuditableBaseEntity
    {
        [Key]
        public int IdBoleto { get; set; }

        public decimal Precio { get; set; }

        public string Estado { get; set; }

        public DateTime FechaCompra { get; set; }

        // Relaciones foráneas
        public int IdViaje { get; set; }
        [ForeignKey("IdViaje")]
        public virtual Viaje Viaje { get; set; }

        public int IdCliente { get; set; }
        [ForeignKey("IdCliente")]
        public virtual Cliente Cliente { get; set; }

        public int IdAsiento { get; set; }
        [ForeignKey("IdAsiento")]
        public virtual Asiento Asiento { get; set; }

        public int IdVendedor { get; set; }
        [ForeignKey("IdVendedor")]
        public virtual Usuario Vendedor { get; set; }
    }
}
