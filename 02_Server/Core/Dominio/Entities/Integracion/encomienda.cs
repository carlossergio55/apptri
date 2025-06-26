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
    [Table("encomienda", Schema = "public")]
    public class Encomienda : AuditableBaseEntity
    {
        [Key]
        public int IdEncomienda { get; set; }
        public string Remitente { get; set; }
        public string Destinatario { get; set; }
        public string Descripcion { get; set; }

        public int IdViaje { get; set; }
        [ForeignKey("IdViaje")]
        public virtual Viaje Viaje { get; set; }
        public decimal Precio { get; set; }
        public string Estado { get; set; } = "en camino";
        public decimal Peso { get; set; }
        public bool Pagado { get; set; } = false;
        public string GuiaCarga { get; set; }
    }
}