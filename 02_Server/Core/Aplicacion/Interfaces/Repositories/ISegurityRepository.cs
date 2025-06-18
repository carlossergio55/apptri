using Aplicacion.DTOs.Segurity;
using System.Threading.Tasks;

namespace Aplicacion.Interfaces.Repositories
{
    public interface ISegurityRepository
    {
        Task<SegUsuarioDto> ValidateUserLoguin(string pUsuario, string pPassword, string pIpAddress);
    }
}
