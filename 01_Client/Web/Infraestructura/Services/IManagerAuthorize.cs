using Infraestructura.Abstract;
using Infraestructura.Models.Authentication;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infraestructura.Services
{
    public interface IManagerAuthorize
    {

        Task LoginAsync(SegResponse response);
        
        Task LogoutnAsync();

    }
}
