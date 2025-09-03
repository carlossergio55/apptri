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
    [Table("usuario", Schema = "public")]
    public class Usuario : AuditableBaseEntity
    {
        [Key]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("nombre")]
        [MaxLength(40)]
        public string Nombre { get; set; }

        [Column("correo")]
        public string Correo { get; set; }

        [Column("contrasena")]
        public string Contraseña { get; set; }

        [Column("tipo")]
        public string Tipo { get; set; }

        // Relación con sucursal (FK)
        [Column("id_sucursal")]
        public int? IdSucursal { get; set; }   // Nullable por ahora
        [ForeignKey("IdSucursal")]
        public virtual Sucursal Sucursal { get; set; }
    }
}
