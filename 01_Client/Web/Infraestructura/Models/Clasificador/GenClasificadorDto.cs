using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Models.Clasificador
{
    public class GenClasificadorDto
    {
        public int IdgenClasificadortipo { get; set; }
        public int IdgenClasificador { get; set; }
        public string Descripcion { get; set; }
        public string? Valor2 { get; set; }
        public string? Valor3 { get; set; }
        public int? Valor4 { get; set; }
        public int? Valor5 { get; set; }
        public bool VerDetalle { get; set; }
    }
}
