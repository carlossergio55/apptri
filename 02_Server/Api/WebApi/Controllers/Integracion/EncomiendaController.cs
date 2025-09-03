using Aplicacion.Features.Integracion.Commands.EncomiendaC;
using Aplicacion.Features.Integracion.Queries;
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
        [HttpGet("GetAllGuia")]
        public async Task<IActionResult> GetAllEncomiendas([FromQuery] string? guiacarga)
    => Ok(await Mediator.Send(new GetAllEncomiendaGuiaQuery(guiacarga)));

        [HttpGet("encomienda")]
        [Authorize]
        public async Task<IActionResult> Get() =>
            Ok(await Mediator.Send(new GetAllEncomiendaQuery()));

        [HttpPost("guardar")]
        [Authorize]
        public async Task<IActionResult> Post(CreateEncomiendaCommand command) =>
            Ok(await Mediator.Send(command));

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateEncomiendaCommand command)
        {
            if (id != command.IdEncomienda) return BadRequest();
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id) =>
            Ok(await Mediator.Send(new DeleteEncomiendaCommand { IdEncomienda = id }));
    }
}
