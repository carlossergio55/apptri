using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models.Clasificador
{
    public class GenClasificadorTipoDto
    {
        public int IdgenClasificadortipo { get; set; }
        public string Descripcion { get; set; }
        public string Valor1 { get; set; }
        public string UsuarioCreacion { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
