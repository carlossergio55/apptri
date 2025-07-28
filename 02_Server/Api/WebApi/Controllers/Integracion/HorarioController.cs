using Aplicacion.Features.Integracion.Commands.HorarioC;
using Aplicacion.Features.Integracion.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.Integracion
{
    [ApiVersion("1.0")]
    [ApiController]
    public class HorarioController : BaseApiController
    {
        [HttpGet("horario")]
        [Authorize]
        public async Task<IActionResult> Get() =>
            Ok(await Mediator.Send(new GetAllHorarioQuery()));

        [HttpPost("guardar")]
        [Authorize]
        public async Task<IActionResult> Post(CreateHorarioCommand command) =>
            Ok(await Mediator.Send(command));

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateHorarioCommand command)
        {
            if (id != command.IdHorario) return BadRequest();
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id) =>
            Ok(await Mediator.Send(new DeleteHorarioCommand { IdHorario = id }));
    }
}
