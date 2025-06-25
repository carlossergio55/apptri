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
    [Table("ruta", Schema = "public")]
    public partial class Ruta : AuditableBaseEntity
    {
        [Key]
        public int IdRuta { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public string Duracion { get; set; }
        public bool EsExtendido { get; set; }

    }
}
