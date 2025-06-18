using Blazored.LocalStorage;
using Infraestructura.Models.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infraestructura.Services
{
    public class ManagerStateAuthorize : AuthenticationStateProvider, IManagerAuthorize
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly string KEYACCES = "KEYACCES";
        private readonly string USER = "USER";
        private readonly string NAVMENU = "NAVMENU";
        private AuthenticationState anonimo => new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        public ManagerStateAuthorize(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;            
        }

        public async Task LoginAsync(SegResponse respose)
        {
            var _user = new ObjectEntity();
            await _localStorageService.SetItemAsStringAsync(KEYACCES, respose.data.jwToken);            
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(respose.data.jwToken))))));
            _user = respose.data;
          //  _user.jwToken = "";
            await _localStorageService.SetItemAsStringAsync(USER, JsonSerializer.Serialize(_user));
            await _localStorageService.SetItemAsStringAsync(NAVMENU, JsonSerializer.Serialize(respose.Menu));
        }

        public async Task LogoutnAsync()
        {            
            await _localStorageService.RemoveItemAsync(KEYACCES);
            await _localStorageService.RemoveItemAsync(USER);
            await _localStorageService.RemoveItemAsync(NAVMENU);
            NotifyAuthenticationStateChanged(Task.FromResult(anonimo));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);

            if (roles != null)
            {
                if (roles.ToString().Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                    foreach (var parsedRole in parsedRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));

            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {            
            var vToken = await _localStorageService.GetItemAsStringAsync(KEYACCES);
            if (string.IsNullOrEmpty(vToken))
            {
                return anonimo;
            }
            var identity = new ClaimsIdentity(ParseClaimsFromJwt(vToken),"Acceso");
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
        }

    }
}
