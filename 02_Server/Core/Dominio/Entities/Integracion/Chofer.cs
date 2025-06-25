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
    [Table("chofer", Schema = "public")]
    public partial class Chofer : AuditableBaseEntity
    {
        [Key]
        public int IdChofer { get; set; }
        public string Nombre { get; set; }
        public int Carnet { get; set; }
        public int Celular { get; set; }
        public string Licencia { get; set; }

    }
}
