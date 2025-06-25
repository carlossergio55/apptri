using Aplicacion.Features.Integracion.Commands;
using Aplicacion.Features.Integracion.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.Integracion
{
    [ApiVersion("1.0")]
    [ApiController]
    public class BusController : BaseApiController
    {

        [HttpGet("bus")]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            return Ok(await Mediator.Send(new GetAllBusQuery()));
        }
        [HttpPost("Guardar")]
        [Authorize]
        public async Task<IActionResult> Post(CreateBusCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteBusCommand { IdBus = id }));
        }
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateBusCommand command)
        {
            if (id != command.IdBus)
            {
                return BadRequest();
            }
            return Ok(await Mediator.Send(command));
        }

    }
}
