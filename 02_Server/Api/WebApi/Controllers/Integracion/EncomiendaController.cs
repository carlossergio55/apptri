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
    public class EncomiendaController : BaseApiController
    {
        [HttpGet("encomienda")]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            return Ok(await Mediator.Send(new GetAllEncomiendaQuery()));
        }

        [HttpPost("guardar")]
        [Authorize]
        public async Task<IActionResult> Post(CreateEncomiendaCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
