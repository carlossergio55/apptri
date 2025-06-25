using Dominio.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entities.Integracion
{
    [Table("bus", Schema = "public")]
    public partial class Bus: AuditableBaseEntity
    {
        [Key]
        public int IdBus { get; set; }
        public string Placa { get; set; }
        public string Modelo { get; set; }
        public int Capacidad { get; set; }

    }
}
