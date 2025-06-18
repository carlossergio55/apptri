using Blazored.SessionStorage;
using Infraestructura.Models.Authentication;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Server.Models
{
    public class MenuService
    {
        public async Task<IEnumerable<SegMenuResponse>> GetMenuData(ISessionStorageService sessionStorageService)
        {
            IEnumerable<SegMenuResponse> menuData;
            menuData= await sessionStorageService.GetItemAsync<List<SegMenuResponse>>("SessionMenu").ConfigureAwait(false);

            return menuData;
        }
    }
}
