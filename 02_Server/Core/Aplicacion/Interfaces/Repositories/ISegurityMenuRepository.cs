using Aplicacion.DTOs.Segurity;
using Aplicacion.Wrappers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aplicacion.Interfaces.Repositories
{
    public interface ISegurityMenuRepository
    {
       Task<Response<List<UserMenuDto>>> ValidateUserMenu(int pIdPerfil, int pIdSistema);
    }
}
