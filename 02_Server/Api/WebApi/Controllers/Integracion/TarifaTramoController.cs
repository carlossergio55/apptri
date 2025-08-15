using Aplicacion.Features.Integracion.Commands.TarifaTramoC;
using Aplicacion.Features.Integracion.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.Integracion
{
    [ApiVersion("1.0")]
    [ApiController]
    public class TarifaTramoController : BaseApiController
    {
        // GET por ruta, y opcionalmente por origen/destino (querystring)
        // Ej: GET /tarifa-tramo?idRuta=1&origenParadaId=10&destinoParadaId=20
        [HttpGet("tarifa-tramo")]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery] int idRuta, [FromQuery] int? origenParadaId, [FromQuery] int? destinoParadaId) =>
            Ok(await Mediator.Send(new GetTarifaTramoQuery
            {
                IdRuta = idRuta,
                OrigenParadaId = origenParadaId,
                DestinoParadaId = destinoParadaId
            }));

        // Crear tarifa
        [HttpPost("guardar")]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] CreateTarifaTramoCommand command) =>
            Ok(await Mediator.Send(command));

        // Actualizar precio de un tramo (PK compuesta en ruta)
        [HttpPut("{idRuta}/{origenParadaId}/{destinoParadaId}")]
        [Authorize]
        public async Task<IActionResult> Put(int idRuta, int origenParadaId, int destinoParadaId, [FromBody] UpdateTarifaTramoCommand command)
        {
            if (idRuta != command.IdRuta || origenParadaId != command.OrigenParadaId || destinoParadaId != command.DestinoParadaId)
                return BadRequest("Ids de ruta no coinciden con el payload.");
            return Ok(await Mediator.Send(command));
        }

        // Eliminar tarifa de tramo
        [HttpDelete("{idRuta}/{origenParadaId}/{destinoParadaId}")]
        [Authorize]
        public async Task<IActionResult> Delete(int idRuta, int origenParadaId, int destinoParadaId) =>
            Ok(await Mediator.Send(new DeleteTarifaTramoCommand
            {
                IdRuta = idRuta,
                OrigenParadaId = origenParadaId,
                DestinoParadaId = destinoParadaId
            }));
    }
}
