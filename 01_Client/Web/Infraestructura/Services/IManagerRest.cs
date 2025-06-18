using Infraestructura.Abstract;
using Infraestructura.Models.Authentication;
using System.Threading.Tasks;

namespace Infraestructura.Services
{
    public interface IManagerRest
    {

        Task<ResponseEntity<T>> GetAsyncFromQuery<T>(string pControlador, object parametros);
        Task<ResponseEntity<T>> GetAsyncFromPath<T>(string pControlador, object parametros);
        Task<ResponseEntity<T>> GetAsync<T>(string pControlador);

        Task<ResponseEntity<T>> PostAsync<T>(string pControlador, object parametros);
        Task<ResponseEntity<T>> DeleteAsync<T>(string pControlador, int id);
        Task<ResponseEntity<T>> PutAsync<T>(string pControlador, object parametros, int id);
                       
        Task<SegResponse> PostAsync(string pControlador, LoginRequest vmodel);

    }
}
