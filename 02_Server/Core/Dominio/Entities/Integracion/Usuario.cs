using Dominio.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; // si quieres ocultar en JSON

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
        public string? Nombre { get; set; }

        [Column("correo")]
        [MaxLength(100)]
        public string? Correo { get; set; }

        // BD: contrasena (nullable) | UI: "Contraseña"
        [Column("contrasena")]
        [Display(Name = "Contraseña")]
        [JsonIgnore] // Opcional: no exponer en respuestas JSON
        public string? Contrasena { get; set; }

        [Column("tipo")]
        [MaxLength(20)]
        public string? Tipo { get; set; }

        [Column("id_sucursal")]
        public int? IdSucursal { get; set; }

        [ForeignKey(nameof(IdSucursal))]
        public virtual Sucursal? Sucursal { get; set; }
    }
}
