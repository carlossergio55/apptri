using Aplicacion.DTOs.Integracion;
using Aplicacion.Features.Integracion.Commands.ParadaC;
using Aplicacion.Features.Integracion.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.Integracion
{
    [ApiVersion("1.0")]
    [ApiController]
    public class ParadaController : BaseApiController
    {
        [HttpGet("parada")]
        [Authorize]
        public async Task<IActionResult> Get() =>
            Ok(await Mediator.Send(new GetAllParadaQuery()));

        [HttpPost("guardar")]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] CreateParadaCommand command) =>
            Ok(await Mediator.Send(command));

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateParadaCommand command)
        {
            if (id != command.IdParada) return BadRequest("Id de ruta y payload no coinciden.");
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id) =>
            Ok(await Mediator.Send(new DeleteParadaCommand { IdParada = id }));
    }
}
