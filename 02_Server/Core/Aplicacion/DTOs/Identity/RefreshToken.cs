using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Identity
{
    public class RefreshToken
    {

        public int id { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.Now >= Expires;
        public DateTime Created { get; set; }
        public string CreateByIp { get; set; }
        public DateTime? Revoked { get; set; }
        public string RevokeByIp { get; set; }
        public string ReplaceByToken { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;





    }
}
