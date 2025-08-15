
using Aplicacion.Features.Integracion.Commands.GuiaCargaC;
using Aplicacion.Features.Integracion.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.Integracion
{
    [ApiVersion("1.0")]
    [ApiController]
    public class GuiaCargaController : BaseApiController
    {
        [HttpGet("guia-carga")]
        [Authorize]
        public async Task<IActionResult> Get() =>
            Ok(await Mediator.Send(new GetAllGuiaCargaQuery()));

        [HttpPost("guardar")]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] CreateGuiaCargaCommand command) =>
            Ok(await Mediator.Send(command));

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateGuiaCargaCommand command)
        {
            if (id != command.IdGuiaCarga) return BadRequest("Id de ruta y payload no coinciden.");
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id) =>
            Ok(await Mediator.Send(new DeleteGuiaCargaCommand { IdGuiaCarga = id }));
    }
}
