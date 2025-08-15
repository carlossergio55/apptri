using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Entities.Integracion
{
    [Table("guia_carga", Schema = "public")]
    public class GuiaCarga
    {
        [Key]
        [Column("id_guia_carga")]
        public int IdGuiaCarga { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("codigo")]
        public string Codigo { get; set; } = null!;   // p.ej. SAM-0045

        // (Opcional) navegación inversa
        public virtual ICollection<Encomienda> Encomiendas { get; set; } = new List<Encomienda>();
    }
}
