using Aplicacion.DTOs.Identity;
using Aplicacion.DTOs.Segurity;
using Aplicacion.Exceptions;
using Aplicacion.Interfaces.Repositories;
using AutoMapper;
using Dominio.Settings;
using Identity.Context;
using Identity.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Helpers;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Persistencia.Repository.Common.Seguridad
{
    public class SegurityRepository : ISegurityRepository
    {
        private readonly SegurityContext _context;
        private readonly IMapper _mapper;
        private readonly JWTSettings _JWTSettings;

        public SegurityRepository(SegurityContext context, IMapper mapper, IOptions<JWTSettings> jWTSettings)
        {
            _context = context;
            _mapper = mapper;
            _JWTSettings = jWTSettings.Value;
        }

        public async Task<SegUsuarioDto> ValidateUserLoguin(string pUsuario, string pPassword, string pIpAddress)
        {
            var usuario = await Task.FromResult(_context.Usuario
                                .Where(f => f.LoginUsuario.ToUpper() == pUsuario.ToUpper()
                                        && f.Password == CGeneric.MD5Hash(pPassword) && f.IdcEstado == ((int)Helpers.ESTADO_CUENTA.ACTIVO).ToString())
                                .FirstOrDefault());

            if (usuario == null) {
                throw new ApiException("Usuario o Password Incorrecto");
            }

            var user = _mapper.Map<SegUsuarioDto>(usuario);
            user.JwToken = new JwtSecurityTokenHandler().WriteToken(await GenerateJwtToken(user));

            var refreshToken = GenerateRefreshToken(pIpAddress);
            user.RefreshToken = refreshToken.Token;

            return user;
        }

        private async Task<JwtSecurityToken> GenerateJwtToken(SegUsuarioDto user)
        {

            //  var userClaims = await _userManager.GetClaimsAsync(user);
            //  var roles = await _userManager.GetRolesAsync(user);

            //var roleClaims = new List<Claim>();

            //for (int i = 0; i < roles.Count; i++)
            //{
            //    roleClaims.Add(new Claim("roles", roles[i]));
            //}
            string ipAddres = IpHelper.GetIpAddress();
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.NombreCompleto),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),              
                new Claim("uid", user.IdsegUsuarioSistema.ToString()),
                new Claim("ip", ipAddres),
                new Claim("Institucion", user.Institucion),
                new Claim("sucursal", user.Sucursal),
                new Claim("IdInstitucion", user.IdgenInstitucion.ToString()),
                new Claim("IdSucursal", user.IdgenInstitucionsucursal.ToString()),
                new Claim("Loguin", user.LoginUsuario),
                new Claim("Perfil", user.Perfil ),
                new Claim("IdPerfil", user.IdsegPerfil.ToString()),
                new Claim("Estado", user.Estado),
                new Claim("Expedido", user.Espedido),
                new Claim("NroCI", user.NroCi)
            }
            //.Union(userClaims)
            //.Union(roleClaims)
            ;

            var symetricSecurity = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_JWTSettings.Key));
            var signingCredencial = new SigningCredentials(symetricSecurity, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                                                issuer: _JWTSettings.Issuer,
                                                audience: _JWTSettings.Audience,
                                                claims: claims,
                                                expires: DateTime.Now.AddMinutes(_JWTSettings.DurationInMinutes),
                                                signingCredentials: signingCredencial);
            return  await Task.FromResult(jwtSecurityToken);
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
