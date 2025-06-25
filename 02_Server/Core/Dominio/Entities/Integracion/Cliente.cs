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
    [Table("cliente", Schema = "public")]
    public partial class Cliente : AuditableBaseEntity
    {
        [Key]
        public int IdCliente { get; set; }
        public string Nombre { get; set; }
        public int Carnet { get; set; }
        public string Correo { get; set; }
        public int Celular { get; set; }

    }
}
