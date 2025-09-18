using Aplicacion.Features.Integracion.Commands.ViajeC;
using Aplicacion.Features.Integracion.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.Integracion
{
    [ApiVersion("1.0")]
    [ApiController]
    public class ViajeController : BaseApiController
    {
        [HttpGet("viaje")]
        [Authorize]
        public async Task<IActionResult> Get() =>
            Ok(await Mediator.Send(new GetAllViajeQuery()));

        [HttpPost("guardar")]
        [Authorize]
        public async Task<IActionResult> Post(CreateViajeCommand command) =>
            Ok(await Mediator.Send(command));

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateViajeCommand command)
        {
            if (id != command.IdViaje) return BadRequest();
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id) =>
            Ok(await Mediator.Send(new DeleteViajeCommand { IdViaje = id }));

        [HttpGet("{id}/paradas")]
        [Authorize]
        public async Task<IActionResult> GetParadas(int id)
        {
            var data = await Mediator.Send(new GetParadasDeViajeQuery { IdViaje = id });
            if (data.Count == 0) return NotFound();
            return Ok(data);
        }


        [HttpPost("generar-proximos")]
        [Authorize]
        public async Task<IActionResult> GenerarProximos([FromBody] GenerarViajesProximosCommand cmd)
    => Ok(await Mediator.Send(cmd));

        [HttpPost("actualizar-estados")]
        [Authorize]
        public async Task<IActionResult> ActualizarEstados([FromBody] ActualizarEstadosViajeCommand cmd)
    => Ok(await Mediator.Send(cmd));


    }
}
