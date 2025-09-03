using Dominio.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Entities.Integracion
{
    [Table("encomienda", Schema = "public")]
    public class Encomienda : AuditableBaseEntity
    {
        [Key]
        [Column("id_encomienda")]
        public int IdEncomienda { get; set; }

        [Column("remitente"), MaxLength(40)]
        public string Remitente { get; set; } = string.Empty;

        [Column("destinatario"), MaxLength(40)]
        public string Destinatario { get; set; } = string.Empty;

        [Column("descripcion")]
        public string? Descripcion { get; set; }
        public string? Guiacarga { get; set; }

        [Column("id_viaje")]
        public int IdViaje { get; set; }
        [ForeignKey(nameof(IdViaje))]
        public virtual Viaje Viaje { get; set; } = null!;

        [Column("precio", TypeName = "numeric(10,2)")]
        public decimal Precio { get; set; }

        [Column("estado"), MaxLength(20)]
        public string Estado { get; set; } = "en camino";

        [Column("peso", TypeName = "numeric(10,2)")]
        public decimal Peso { get; set; }

        [Column("pagado")]
        public bool Pagado { get; set; } = false;

        // ===== FK a GUIA_CARGA =====
        public virtual GuiaCarga Guia { get; set; } = null!;

        // ===== Tramo (origen/destino) =====
        [Column("origen_parada_id")]
        public int? OrigenParadaId { get; set; }
        [ForeignKey(nameof(OrigenParadaId))]
        public virtual Parada? OrigenParada { get; set; }

        [Column("destino_parada_id")]
        public int? DestinoParadaId { get; set; }
        [ForeignKey(nameof(DestinoParadaId))]
        public virtual Parada? DestinoParada { get; set; }
    }
}
