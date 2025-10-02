// Dominio.Entities.Integracion.TarifaTramo.cs
using Dominio.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Entities.Integracion
{
    [Table("tarifa_tramo", Schema = "public")]
    public class TarifaTramo : AuditableBaseEntity
    {
        [Column("id_tarifa_tramo")]
        public int IdTarifaTramo { get; set; }   

        [Column("id_ruta")]
        public int IdRuta { get; set; }

        [Column("origen_parada_id")]
        public int OrigenParadaId { get; set; }

        [Column("destino_parada_id")]
        public int DestinoParadaId { get; set; }

        [Column("precio", TypeName = "numeric(10,2)")]
        public decimal Precio { get; set; }

        public virtual Ruta Ruta { get; set; } = null!;
        public virtual Parada OrigenParada { get; set; } = null!;
        public virtual Parada DestinoParada { get; set; } = null!;
    }
}
