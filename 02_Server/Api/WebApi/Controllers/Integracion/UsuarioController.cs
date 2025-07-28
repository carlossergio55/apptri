using Aplicacion.Features.Integracion.Commands;

using Aplicacion.Features.Integracion.Commands.UsuarioC;
using Aplicacion.Features.Integracion.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.Integracion
{
    [ApiVersion("1.0")]
    [ApiController]
    public class UsuarioController : BaseApiController
    {

        [HttpGet("usuario")]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            return Ok(await Mediator.Send(new GetAllUsuarioQuery()));
        }

        [HttpPost("Guardar")]
        [Authorize]
        public async Task<IActionResult> Post(CreateUsuarioCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteUsuarioCommand { IdUsuario = id }));
        }
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateUsuarioCommand command)
        {
            if (id != command.IdUsuario)
            {
                return BadRequest();
            }
            return Ok(await Mediator.Send(command));
        }

    }

}
