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
        // ============================
        //   PÚBLICO (rastreo por guía)
        //   GET /api/v1/Encomienda/track/{guiacarga}
        //   GET /api/v1/Encomienda/track?guiacarga=ABC123
        // ============================
        [HttpGet("track/{guiacarga?}")]
        [AllowAnonymous]
        public async Task<IActionResult> Track(string? guiacarga)
        {
            if (string.IsNullOrWhiteSpace(guiacarga))
                return BadRequest("guiacarga requerida.");

            return Ok(await Mediator.Send(new EncomiendaGuiaQuery { Guiacarga = guiacarga }));
        }


        // ============================
        //   ADMIN (listado / gestión)
        //   GET /api/v1/Encomienda/encomienda?guiacarga=...
        // ============================
        [HttpGet("encomienda")]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery] string? guiacarga = null)
            => Ok(await Mediator.Send(new GetAllEncomiendaQuery { Guiacarga = guiacarga }));

        [HttpPost("guardar")]
        [Authorize]
        public async Task<IActionResult> Post(CreateEncomiendaCommand command)
            => Ok(await Mediator.Send(command));

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateEncomiendaCommand command)
        {
            if (id != command.IdEncomienda) return BadRequest();
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
            => Ok(await Mediator.Send(new DeleteEncomiendaCommand { IdEncomienda = id }));
    }
}
