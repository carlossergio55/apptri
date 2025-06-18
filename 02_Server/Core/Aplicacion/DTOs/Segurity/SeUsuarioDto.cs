using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Segurity
{
    public class SeUsuarioDto
    {
        public int IdsegUsuarioSistema { get; set; }

        public string IdSistemaAlterno { get; set; }
        public int? IdsegPerfil { get; set; }

        public string LoginUsuario { get; set; }
        public string Password { get; set; }
        public string NroCi { get; set; }

        public int? IdcTipousuario { get; set; }
        public int IdgenInstitucionsucursal { get; set; }
        public char? IdcEstado { get; set; }
    }
}
