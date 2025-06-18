using Aplicacion.DTOs.Segurity;
using System.Threading.Tasks;

namespace Aplicacion.Interfaces.Repositories
{
    public interface ISeguritySystemRepository
    {
        Task<SegUsuarioDto> ValidateUserSystemLoguin(string pUsuario, string pIpAddress);
    }
}
