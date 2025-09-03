using Dominio.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Entities.Integracion
{
    [Table("ruta_parada", Schema = "public")]
    public partial  class RutaParada : AuditableBaseEntity
    {
        [Key]
        public int IdRutaParada { get; set; }

        public int IdRuta { get; set; }

        public int IdParada { get; set; }

        public int Orden { get; set; }

        public virtual Ruta Ruta { get; set; }

        public virtual Parada Parada { get; set; }
    }
}
