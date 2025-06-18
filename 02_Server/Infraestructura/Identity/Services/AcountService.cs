using Aplicacion.DTOs.Identity;
using Aplicacion.Enums;
using Aplicacion.Exceptions;
using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Settings;
using Identity.Helpers;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Services
{
    public class AcountService : IAcountService
    {

        private readonly UserManager<AplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _RoleManager;
        private readonly SignInManager<AplicationUser> _SignInManager;
        private readonly JWTSettings _JWTSettings;
        private readonly IDateTimeService _dateTimeService;

        public AcountService(UserManager<AplicationUser> userManager, RoleManager<IdentityRole> roleManager,
                            SignInManager<AplicationUser> signInManager, IOptions<JWTSettings> jWTSettings, IDateTimeService dateTimeService)
        {
            _userManager = userManager;
            _RoleManager = roleManager;
            _SignInManager = signInManager;
            _JWTSettings = jWTSettings.Value;
            _dateTimeService = dateTimeService;
        }

        public async Task<Response<AuthenticationResponse>> AuthenticateAsync(AuthenticatioRequest request, string ipAddress)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                throw new ApiException($" Credenciales de acceso incorrectas ");
            }

            var result = await _SignInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                throw new ApiException($" Credenciales de acceso incorrectas ");
            }

            JwtSecurityToken jwtSecurityToken = await GenerateJwtToken(user);
            AuthenticationResponse response = new AuthenticationResponse();
            response.Id = user.Id;
            response.JwToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            //response.UserName = user.Nombre;
            response.Email = user.Email;


            var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            response.Roles = rolesList.ToList();
            response.IsVerified = user.EmailConfirmed;

            var refreshToken = GenerateRefreshToken(ipAddress);
            response.RefreshToken = refreshToken.Token;
            return new Response<AuthenticationResponse>(response, $"Autenticado {user.NombreCompleto}");

        }

        public async Task<Response<string>> RegisterAsync(RegisterRequest request, string origin)
        {
            var usuarioConElmismoUserName = await _userManager.FindByNameAsync(request.UserName);
            if (usuarioConElmismoUserName != null)
            {
                throw new ApiException($"El nombre del usuario  con el nombre {request.UserName} ya fue registraod ");
            }

            var user = new AplicationUser
            {
                Email = request.Email,
            //    Nombre = request.Nombre,
            //    Apellido = request.Apellido,
                UserName = request.UserName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };


            var usuarioConElMismoCorreo = await _userManager.FindByEmailAsync(request.Email);
            if (usuarioConElMismoCorreo != null)
            {
                throw new ApiException($"El nombre del usuario con el mail {request.Email} ya fue registraod ");
            }
            else
            {
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Roles.Basic.ToString());
                    return new Response<string>(user.Id, message: $"Usuario registrado exitosamente {request.UserName}");
                }
                else
                {
                    throw new ApiException($"{result.Errors}");
                }
            }
        }

        private async Task<JwtSecurityToken> GenerateJwtToken(AplicationUser user)
        {

            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }

            string ipAddres = IpHelper.GetIpAddress();

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id),
                new Claim("ip", ipAddres),
            }.Union(userClaims)
            .Union(roleClaims);


            var symetricSecurity = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JWTSettings.Key));
            var signingCredencial = new SigningCredentials(symetricSecurity, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                                                issuer: _JWTSettings.Issuer,
                                                audience: _JWTSettings.Audience,
                                                claims: claims,
                                                expires: DateTime.Now.AddMinutes(_JWTSettings.DurationInMinutes),
                                                signingCredentials: signingCredencial
                                        );

            return jwtSecurityToken;



        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = RandomTokenString(),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now,
                CreateByIp = ipAddress
            };
        }

        private string RandomTokenString()
        {
            using var rngCryptoServicePropider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServicePropider.GetBytes(randomBytes);
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }
    }
}
