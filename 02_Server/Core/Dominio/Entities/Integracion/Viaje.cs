using Dominio.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Entities.Integracion
{
    [Table("viaje", Schema = "public")]
    public class Viaje : AuditableBaseEntity
    {
        [Key]
        [Column("id_viaje")]
        public int IdViaje { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; }

        // TIME(0) en PostgreSQL
        [Column("hora_salida", TypeName = "time(0)")]
        public TimeSpan HoraSalida { get; set; }

        [Column("estado")]
        [MaxLength(20)]
        public string Estado { get; set; } = "Programado";

        // NUEVO: IDA / VUELTA
        [Column("direccion")]
        [MaxLength(10)]
        public string Direccion { get; set; } = "IDA";

        // NUEVO: tramo del viaje
        [Column("desde_parada_id")]
        public int? DesdeParadaId { get; set; }
        [ForeignKey(nameof(DesdeParadaId))]
        public virtual Parada? DesdeParada { get; set; }

        [Column("hasta_parada_id")]
        public int? HastaParadaId { get; set; }
        [ForeignKey(nameof(HastaParadaId))]
        public virtual Parada? HastaParada { get; set; }

        // Relaciones
        [Column("id_ruta")]
        public int IdRuta { get; set; }
        [ForeignKey(nameof(IdRuta))]
        public virtual Ruta Ruta { get; set; } = null!;

        // Hacerlos opcionales para permitir programación sin asignar aún
        [Column("id_chofer")]
        public int? IdChofer { get; set; }
        [ForeignKey(nameof(IdChofer))]
        public virtual Chofer? Chofer { get; set; }

        [Column("id_bus")]
        public int? IdBus { get; set; }
        [ForeignKey(nameof(IdBus))]
        public virtual Bus? Bus { get; set; }
    }
}
