using Blazored.FluentValidation;
using Blazored.LocalStorage;
using Infraestructura.Abstract;
using Infraestructura.Models.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace Server.Pages.Pages.Authentication
{

    public partial class Login
    {
        [Parameter]
        [SupplyParameterFromQuery(Name ="tt")]
        public string tt { get; set; }
        [Parameter]
        [SupplyParameterFromQuery(Name = "sis")]
        public int sis { get; set; }

        private LoginRequest _tokenModel = new();
        private FluentValidationValidator _fluentValidationValidator;
        bool PasswordVisibility;
        InputType PasswordInput = InputType.Password;
        string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        public List<SegMenuResponse> _menuLista = new List<SegMenuResponse>();
        public ObjectEntity _usuarioLog = new ObjectEntity();
        public ObjectEntity _usuarioLogv1 = new ObjectEntity();
        private readonly ILocalStorageService _localStorageService;
        private readonly string USER = "USER";
        private readonly string KEYACCES = "KEYACCES";


        protected override async void OnInitialized()
        {
            if (tt != "" && tt != null)
            {
                _Loading.Show();
                var keyValuePairs = ParseClaimsFromJwt(tt).ToList();
                string[] _usuario = keyValuePairs[8].ToString().Split(':');
                _tokenModel.UserName = _usuario[1].Replace('"', ' ').Trim();
                var resultado = await _Rest.PostAsync("Identity/autenticateSystem", _tokenModel);
                var resultt = await _Rest.GetAsyncFromQuery<List<SegMenuResponse>>($"../Identity/getmenu", new { pidsegperfil = resultado.data.idsegPerfil, pidsegSistema = sis });
                resultado.Menu = resultt.Data;
                await _LoginService.LoginAsync(resultado);
                _Loading.Hide();
                _navMgr.NavigateTo("/dashboard", true);



            }
        }


            async void SubmitAsync()
        {
           
  
            _Loading.Show();
            var result = await _Rest.PostAsync("Identity/autenticate", _tokenModel);
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                _MessageShow(result.message, State.Error);
                _DialogShow(result.message, State.Error);
                _Loading.Hide();
            }
            else
            {
                int codi_sistema = Int32.Parse(Configuration["idSistema"]);
                var resultt = await _Rest.GetAsyncFromQuery<List<SegMenuResponse>>($"../Identity/getmenu", new { pidsegperfil = result.data.idsegPerfil, pidsegSistema = codi_sistema});
                result.Menu = resultt.Data;
                await _LoginService.LoginAsync(result);
                _Loading.Hide();
                _navMgr.NavigateTo("/dashboard", true);
            }
         
        }

        void TogglePasswordVisibility()
        {
            if (PasswordVisibility)
            {
                PasswordVisibility = false;
                PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                PasswordInput = InputType.Password;
            }
            else
            {
                PasswordVisibility = true;
                PasswordInputIcon = Icons.Material.Filled.Visibility;
                PasswordInput = InputType.Text;
            }
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


    }
}
