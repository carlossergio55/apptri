using System;
using System.Collections.Generic;

namespace Infraestructura.Models.Authentication
{
    public class ObjectEntity
    {
        public int idsegUsuarioSistema { get; set; }
        public string nombreCompleto { get; set; }
        public string nroCi { get; set; }
        public string espedido { get; set; }
        public int idsegPerfil { get; set; }
        public string perfil { get; set; }
        public string loginUsuario { get; set; }
        public int idgenInstitucionsucursal { get; set; }
        public int idgenInstitucion { get; set; }
        public string institucion { get; set; }
        public string sucursal { get; set; }
        public string estado { get; set; }
        
        public List<string> roles { get; set; }
        public string jwToken { get; set; }

    }
}
