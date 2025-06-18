using Aplicacion.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
namespace Shared.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {            
            LoginUsuario =  httpContextAccessor.HttpContext.User.FindFirst("Loguin").Value;
            IdgenInstitucionsucursal =  Convert.ToInt32(httpContextAccessor.HttpContext.User.FindFirst("IdSucursal").Value);
            
           // IdgenInstitucionsucursal = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        public string LoginUsuario { get; set; }

        public int IdsegUsuarioSistema { get; set; }
        public string NombreCompleto { get; set; }
        public string NroCi { get; set; }
        public string Espedido { get; set; }
        public int IdsegPerfil { get; set; }
        public string Perfil { get; set; }
        public int IdgenInstitucionsucursal { get; set; }
        public int IdgenInstitucion { get; set; }
        public string Institucion { get; set; }
        public string Sucursal { get; set; }
        public string Estado { get; set; }
        public List<string> Roles { get; set; }

    }
}
