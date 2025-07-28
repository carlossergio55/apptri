using Aplicacion.Features.Integracion.Commands;

using Aplicacion.Features.Integracion.Commands.ClienteC;
using Aplicacion.Features.Integracion.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.Integracion
{
    [ApiVersion("1.0")]
    [ApiController]
    public class ClienteController : BaseApiController
    {

        [HttpGet("cliente")]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            return Ok(await Mediator.Send(new GetAllClienteQuery()));
        }

        [HttpPost("Guardar")]
        [Authorize]
        public async Task<IActionResult> Post(CreateClienteCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteClienteCommand { IdCliente = id }));
        }
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateClienteCommand command)
        {
            if (id != command.IdCliente)
            {
                return BadRequest();
            }
            return Ok(await Mediator.Send(command));
        }


    }
}
