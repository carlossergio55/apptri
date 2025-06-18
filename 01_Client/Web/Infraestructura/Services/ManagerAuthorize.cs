using Infraestructura.Models.Authentication;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Infraestructura.Services
{
    internal class ManagerAuthorize : IManagerAuthorize
    {
        public Task<ClaimsPrincipal> CurrentUser()
        {
            throw new NotImplementedException();
        }

        public Task<LoginResponse> Login(LoginRequest model)
        {
            throw new NotImplementedException();
        }

        public Task<LoginResponse> Logout()
        {
            throw new NotImplementedException();
        }

        public Task<string> RefreshToken()
        {
            throw new NotImplementedException();
        }

        public Task<string> TryForceRefreshToken()
        {
            throw new NotImplementedException();
        }

        public Task<string> TryRefreshToken()
        {
            throw new NotImplementedException();
        }
    }
}
