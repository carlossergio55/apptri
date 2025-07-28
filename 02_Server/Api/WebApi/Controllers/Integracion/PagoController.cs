using Aplicacion.Features.Integracion.Commands.PagoC;
using Aplicacion.Features.Integracion.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.Integracion
{
    [ApiVersion("1.0")]
    [ApiController]
    public class PagoController : BaseApiController
    {
        [HttpGet("pago")]
        [Authorize]
        public async Task<IActionResult> Get() =>
            Ok(await Mediator.Send(new GetAllPagoQuery()));

        [HttpPost("guardar")]
        [Authorize]
        public async Task<IActionResult> Post(CreatePagoCommand command) =>
            Ok(await Mediator.Send(command));

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdatePagoCommand command)
        {
            if (id != command.IdPago) return BadRequest();
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id) =>
            Ok(await Mediator.Send(new DeletePagoCommand { IdPago = id }));
    }
}
