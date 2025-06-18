using Blazored.LocalStorage;
using Infraestructura.Models.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Services
{
    public class ManagerStorage : IManagerStorage
    {

        private readonly ILocalStorageService _localStorageService;

        public ManagerStorage(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public async Task<bool> DeleteStorage(string namekey)
        {
            var result = false;
            try
            {
                await _localStorageService.RemoveItemAsync(namekey);
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public async Task<T> GetStorage<T>(string namekey)
        {
            return await _localStorageService.GetItemAsync<T>(namekey);
        }

        public async Task<string> GetStorage(string namekey)
        {
            return await _localStorageService.GetItemAsStringAsync(namekey);
        }

        public async Task<bool> SetStorage<T>(string namekey, T value)
        {
            var result = false;
            try
            {
                await _localStorageService.SetItemAsync<T>(namekey, value);
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public async Task<bool> SetStorage(string namekey, string value)
        {
            var result = false;
            try
            {
                await _localStorageService.SetItemAsStringAsync(namekey, value);
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;

        }

        public async Task<ObjectEntity> DatosUsuario()
        {
            return await _localStorageService.GetItemAsync<ObjectEntity>("USER");
        }
        public async Task<List<SegMenuResponse>> DatosMenu()
        {
            
            return await _localStorageService.GetItemAsync<List<SegMenuResponse>>("NAVMENU");

            


        }

    }

}
