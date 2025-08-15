using Aplicacion.Features.Integracion.Commands.RutaParadaC;
using Aplicacion.Features.Integracion.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.Integracion
{
    [ApiVersion("1.0")]
    [ApiController]
    public class RutaParadaController : BaseApiController
    {
        // Obtener paradas de una ruta (ordenadas)
        [HttpGet("ruta-parada/{idRuta}")]
        [Authorize]
        public async Task<IActionResult> GetByRuta(int idRuta) =>
            Ok(await Mediator.Send(new GetRutaParadaByRutaQuery { IdRuta = idRuta }));

        // Crear relación ruta-parada
        [HttpPost("guardar")]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] CreateRutaParadaCommand command) =>
            Ok(await Mediator.Send(command));

        // Actualizar orden (PK compuesta en ruta)
        [HttpPut("{idRuta}/{idParada}")]
        [Authorize]
        public async Task<IActionResult> Put(int idRuta, int idParada, [FromBody] UpdateRutaParadaCommand command)
        {
            if (idRuta != command.IdRuta || idParada != command.IdParada)
                return BadRequest("Ids de ruta no coinciden con el payload.");
            return Ok(await Mediator.Send(command));
        }

        // Eliminar relación
        [HttpDelete("{idRuta}/{idParada}")]
        [Authorize]
        public async Task<IActionResult> Delete(int idRuta, int idParada) =>
            Ok(await Mediator.Send(new DeleteRutaParadaCommand { IdRuta = idRuta, IdParada = idParada }));
    }
}
