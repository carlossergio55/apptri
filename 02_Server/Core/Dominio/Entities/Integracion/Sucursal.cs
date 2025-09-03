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
    [Table("sucursal", Schema = "public")]
    public class Sucursal : AuditableBaseEntity
    {
        [Key]
        [Column("id_sucursal")]
        public int IdSucursal { get; set; }

        [Column("id_parada")]
        public int IdParada { get; set; }

        [Column("nombre")]
        [MaxLength(80)]
        public string Nombre { get; set; }

        [Column("ciudad")]
        [MaxLength(60)]
        public string Ciudad { get; set; }

        [Column("direccion")]
        [MaxLength(120)]
        public string Direccion { get; set; }

        [Column("telefono")]
        [MaxLength(30)]
        public string Telefono { get; set; }

        [Column("estado")]
        [MaxLength(20)]
        public string Estado { get; set; }

        // Relación inversa
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}
