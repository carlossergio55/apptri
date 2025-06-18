using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Vistas
{
    public class VpacFormularioDto
    {
        public int IdpacFormulario { get; set; }
        public string Objetocontrato { get; set; }
        public string ContratacionDescripcion { get; set; }
        public DateTime? MesProgramadoInicio { get; set; }
        public DateTime? MesProgramadoPublicar { get; set; }
        public string OrganismoFinanciero { get; set; }
        public decimal? PrecioReferencial { get; set; }
        public int? MesPago { get; set; }
        public string AreaDescripcion { get; set; }
        public string UsuarioNombreCompleto { get; set; }
        public string ModalidadDescripcion { get; set; }
        public string PartidaCodigoDescripcion { get; set; }
        public string Codigopac { get; set; }
        public string EstadoAccion { get; set; }
        public string Justificacion { get; set; }
        public string UnidadSolicitante { get; set; }
    }
}
