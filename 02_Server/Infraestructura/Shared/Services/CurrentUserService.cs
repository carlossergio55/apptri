using Aplicacion.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Shared.Services
{
    /// <summary>
    /// Este servicio permite obtener los datos del usuario actual autenticado.
    /// Si el usuario no está autenticado (por ejemplo, en endpoints públicos), se asignan valores por defecto.
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly ILogger<CurrentUserService> _logger;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, ILogger<CurrentUserService> logger)
        {
            _logger = logger;
            var user = httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated == true)
            {
                LoginUsuario = user.FindFirst("Loguin")?.Value ?? "Anonimo";
                IdgenInstitucionsucursal = Convert.ToInt32(user.FindFirst("IdSucursal")?.Value ?? "0");

                // Los siguientes valores son opcionales, asegúrate de agregarlos si los necesitas
                NombreCompleto = user.FindFirst("NombreCompleto")?.Value ?? "";
                NroCi = user.FindFirst("NroCI")?.Value ?? "";
                Espedido = user.FindFirst("Expedido")?.Value ?? "";
                IdsegUsuarioSistema = Convert.ToInt32(user.FindFirst("uid")?.Value ?? "0");
                IdsegPerfil = Convert.ToInt32(user.FindFirst("IdPerfil")?.Value ?? "0");
                Perfil = user.FindFirst("Perfil")?.Value ?? "";
                IdgenInstitucion = Convert.ToInt32(user.FindFirst("IdInstitucion")?.Value ?? "0");
                Institucion = user.FindFirst("Institucion")?.Value ?? "";
                Sucursal = user.FindFirst("sucursal")?.Value ?? "";
                Estado = user.FindFirst("Estado")?.Value ?? "";
                Roles = new List<string>(); // Opcional: podrías mapear si tienes varios roles

                _logger.LogInformation($"Usuario autenticado: {LoginUsuario}");
            }
            else
            {
                // Usuario anónimo
                LoginUsuario = "Anonimo";
                IdgenInstitucionsucursal = 0;
                NombreCompleto = "";
                NroCi = "";
                Espedido = "";
                IdsegUsuarioSistema = 0;
                IdsegPerfil = 0;
                Perfil = "";
                IdgenInstitucion = 0;
                Institucion = "";
                Sucursal = "";
                Estado = "";
                Roles = new List<string>();

                _logger.LogWarning("Se accedió al sistema sin autenticación.");
            }
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

        /// <summary>
        /// Indica si el usuario está autenticado o no.
        /// </summary>
        public bool EstaAutenticado => LoginUsuario != "Anonimo";
    }
}
