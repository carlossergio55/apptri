using Aplicacion.Features.Integracion.Commands.AsientoC;
using Aplicacion.Features.Integracion.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.Integracion
{
    [ApiVersion("1.0")]
    [ApiController]
    public class AsientoController : BaseApiController
    {
        [HttpGet("asiento")]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            return Ok(await Mediator.Send(new GetAllAsientoQuery()));
        }

        [HttpPost("guardar")]
        [Authorize]
        public async Task<IActionResult> Post(CreateAsientoCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteAsientoCommand { IdAsiento = id }));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateAsientoCommand command)
        {
            if (id != command.IdAsiento)
            {
                return BadRequest("El ID de la URL no coincide con el del cuerpo.");
            }

            return Ok(await Mediator.Send(command));
        }
    }
}
