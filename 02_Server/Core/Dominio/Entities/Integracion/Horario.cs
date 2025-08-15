using Dominio.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Entities.Integracion
{
    [Table("horario", Schema = "public")]
    public class Horario : AuditableBaseEntity
    {
        [Key]
        [Column("id_horario")]
        public int IdHorario { get; set; }

        // TIME(0) en PostgreSQL
        [Column("hora_salida", TypeName = "time(0)")]
        public TimeSpan HoraSalida { get; set; }

        // En BD es VARCHAR; si luego lo migras a SMALLINT, cambiamos aquí a short
        [Column("dia_semana")]
        [MaxLength(10)]
        public string DiaSemana { get; set; } = null!; // ej: "0".."6" o "Dom/Lun"

        // NUEVO: IDA / VUELTA
        [Column("direccion")]
        [MaxLength(10)]
        public string Direccion { get; set; } = "IDA";

        // NUEVO: tramo que cubre este horario
        [Column("desde_parada_id")]
        public int? DesdeParadaId { get; set; }

        [ForeignKey(nameof(DesdeParadaId))]
        public virtual Parada? DesdeParada { get; set; }

        [Column("hasta_parada_id")]
        public int? HastaParadaId { get; set; }

        [ForeignKey(nameof(HastaParadaId))]
        public virtual Parada? HastaParada { get; set; }

        // Relación con ruta
        [Column("id_ruta")]
        public int IdRuta { get; set; }

        [ForeignKey(nameof(IdRuta))]
        public virtual Ruta Ruta { get; set; } = null!;
    }
}
