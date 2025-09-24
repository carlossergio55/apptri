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

        [MaxLength(20)]
        [Column("estado")]
        public string Estado { get; set; } = "BLOQUEADO";

        [Column("fecha_reserva_utc")]
        public DateTime? FechaReservaUtc { get; set; }

        [Column("fecha_confirmacion_utc")]
        public DateTime? FechaConfirmacionUtc { get; set; }

        // (Opcional) Si sigues teniendo la columna fecha_compra y quieres mantenerla mapeada:
        [Column("fecha_compra")]
        public DateTime? FechaCompra { get; set; }

        // --- Relaciones ---

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

        [Column("id_pago")]
        public int? IdPago { get; set; }
        [ForeignKey(nameof(IdPago))]
        public virtual Pago? Pago { get; set; }

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
