using Aplicacion.Features.Integracion.Commands;
using Aplicacion.Features.Integracion.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.Integracion
{
    [ApiVersion("1.0")]
    [ApiController]
    //[Route("api/v{version:apiVersion}/[controller]")]
    public class PagoController : BaseApiController
    {
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            return Ok(await Mediator.Send(new GetAllPagoQuery()));
        }

        [HttpPost("crear")]
        [Authorize]
        public async Task<IActionResult> Crear([FromBody] CreatePagoCommand command)
        {
            var resultado = await Mediator.Send(command);
            return Ok(resultado);
        }
    }
}
