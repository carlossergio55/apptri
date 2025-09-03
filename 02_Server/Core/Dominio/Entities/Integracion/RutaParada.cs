using Dominio.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Entities.Integracion
{
   
    public class RutaParada : AuditableBaseEntity
    {
        [Key]
        [Column("id_ruta_parada")]
        public int IdRutaParada { get; set; }

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
