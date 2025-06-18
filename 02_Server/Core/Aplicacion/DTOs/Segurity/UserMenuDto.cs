using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Segurity
{
    public class UserMenuDto
    {
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
