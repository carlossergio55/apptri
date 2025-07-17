using Aplicacion.DTOs.Identity;
using Aplicacion.Features.Segurity.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Webapi.Controllers.v1.Identity
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : BaseApiController
    {
        [HttpPost("autenticate")]
        public async Task<IActionResult> AuthenticateAsync(AuthenticatioRequest request)
        {
            return Ok(await Mediator.Send(new SegurityAuthenticateUserCommand
            {
                Usuario = request.UserName,
                Password = request.Password,
                IpAddress = GenerateIpAddress()
            }));
        }

        [HttpPost("autenticateSystem")]
        public async Task<IActionResult> AuthenticateSystemAsync(AuthenticatioRequest request)
        {
            return Ok(await Mediator.Send(new SegurityAuthenticateUserSystemCommand
            {
                Usuario = request.UserName,
                IpAddress = GenerateIpAddress()
            }));
        }


        [HttpGet("getmenu")]
        [Authorize]
        public async Task<IActionResult> getmenu(int pidsegperfil, int pidsegSistema)
        {
            return Ok(await Mediator.Send(new GetAllUserMenuQuery
            {
                idsegperfil = pidsegperfil,
                idsegSistema = pidsegSistema,
            }));
        }
  

        private string GenerateIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Fowarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
