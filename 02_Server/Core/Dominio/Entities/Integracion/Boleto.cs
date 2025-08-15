using Dominio.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Entities.Integracion
{
    [Table("boleto", Schema = "public")]
    public class Boleto : AuditableBaseEntity
    {
        [Key]
        [Column("id_boleto")]
        public int IdBoleto { get; set; }

        [Column("precio", TypeName = "numeric(10,2)")]
        public decimal Precio { get; set; }

        [Column("estado")]
        [MaxLength(20)]
        public string Estado { get; set; } = "Pagado";

        [Column("fecha_compra")]
        public DateTime FechaCompra { get; set; } = DateTime.Now;

        // ===== Relaciones foráneas =====
        [Column("id_viaje")]
        public int IdViaje { get; set; }
        [ForeignKey(nameof(IdViaje))]
        public virtual Viaje Viaje { get; set; } = null!;

        [Column("id_cliente")]
        public int IdCliente { get; set; }
        [ForeignKey(nameof(IdCliente))]
        public virtual Cliente Cliente { get; set; } = null!;

        [Column("id_asiento")]
        public int IdAsiento { get; set; }
        [ForeignKey(nameof(IdAsiento))]
        public virtual Asiento Asiento { get; set; } = null!;

        // (Opcional) pago asociado
        [Column("id_pago")]
        public int? IdPago { get; set; }
        [ForeignKey(nameof(IdPago))]
        public virtual Pago? Pago { get; set; }

        // ===== NUEVO: Tramo (origen/destino) =====
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
