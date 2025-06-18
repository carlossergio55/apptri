using Dominio.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Entities.Seguridad
{
    [Table("segv_menu_objetos", Schema = "public")]
    public class SegvMenuobjetos : AuditableBaseEntity
    {
        [Key]
        public int id { get; set; }
        public int parentid { get; set; }
        public string texto { get; set; }
        public string url { get; set; }
        public int idseg_sistema { get; set; }
        public string nombre_sistema { get; set; }
        public int idseg_perfil { get; set; }
        public string nombre_perfil { get; set; }
        public int posicion_modulo { get; set; }
        public string menu { get; set; }
        public string IconoModulo { get; set; }
    }
}
