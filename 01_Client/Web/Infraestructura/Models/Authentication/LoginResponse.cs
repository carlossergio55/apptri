using Infraestructura.Abstract;
using System;

namespace Infraestructura.Models.Authentication
{
    public class LoginResponse : ResponseBase
    {
        public string Token { get; set; }
        public string Nombre { get; set; }
        public DateTime? Vigencia { get; set; }
    }
}
