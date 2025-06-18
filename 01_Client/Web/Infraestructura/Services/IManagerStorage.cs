using Infraestructura.Models.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Services
{
    public interface IManagerStorage
    {

        Task<bool> SetStorage<T>(string nanekey, T value);
        Task<bool> SetStorage(string nanekey, string value);

        Task<T> GetStorage<T>(string namekey);

        Task<bool> DeleteStorage(string namekey);

        Task<ObjectEntity> DatosUsuario();
        Task<List<SegMenuResponse>> DatosMenu();
    }
}
