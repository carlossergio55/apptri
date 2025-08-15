using Dominio.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Entities.Integracion
{
    [Table("ruta_parada", Schema = "public")]
    public class RutaParada : AuditableBaseEntity
    {
        [Column("id_ruta")]
        public int IdRuta { get; set; }

        [Column("id_parada")]
        public int IdParada { get; set; }

        [Column("orden")]
        public int Orden { get; set; }

       
        [ForeignKey(nameof(IdRuta))]
        public virtual Ruta Ruta { get; set; } = null!;

        [ForeignKey(nameof(IdParada))]
        public virtual Parada Parada { get; set; } = null!;
    }
}
