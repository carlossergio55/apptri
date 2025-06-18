using Aplicacion.DTOs.Identity;
using Aplicacion.Wrappers;
using System.Threading.Tasks;

namespace Aplicacion.Interfaces
{
    public interface IAcountService
    {
        Task<Response<AuthenticationResponse>> AuthenticateAsync(AuthenticatioRequest request, string ipAddress);
        Task<Response<string>> RegisterAsync(RegisterRequest request, string origin);
    }
}
