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
    public class RutaController : BaseApiController
    {

        [HttpGet("ruta")]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            return Ok(await Mediator.Send(new GetAllRutaQuery()));
        }
        [HttpPost("Guardar")]
        [Authorize]
        public async Task<IActionResult> Post(CreateRutaCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteRutaCommand { IdRuta = id }));
        }
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateRutaCommand command)
        {
            if (id != command.IdRuta)
            {
                return BadRequest();
            }
            return Ok(await Mediator.Send(command));
        }


    }
}

