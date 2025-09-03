using Aplicacion.Features.Integracion.Commands.SucursalC;
using Aplicacion.Features.Integracion.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.Integracion
{
    [ApiVersion("1.0")]
    [ApiController]
    public class SucursalController : BaseApiController
    {
        // GET: api/v1/Sucursal/sucursal
        [HttpGet("sucursal")]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            return Ok(await Mediator.Send(new GetAllSucursalQuery()));
        }

        // POST: api/v1/Sucursal/Guardar
        [HttpPost("Guardar")]
        [Authorize]
        public async Task<IActionResult> Post(CreateSucursalCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        // PUT: api/v1/Sucursal/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateSucursalCommand command)
        {
            if (id != command.IdSucursal)
                return BadRequest("El id de la ruta no coincide con el del cuerpo.");

            return Ok(await Mediator.Send(command));
        }

        // DELETE: api/v1/Sucursal/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteSucursalCommand { IdSucursal = id }));
        }
    }
}
