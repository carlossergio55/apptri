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
        // GET listado/filtrado
        [HttpGet("tarifa-tramo")]
        [Authorize]
        public async Task<IActionResult> Get(
            [FromQuery] int idRuta,
            [FromQuery] int? origenParadaId,
            [FromQuery] int? destinoParadaId) =>
            Ok(await Mediator.Send(new GetTarifaTramoQuery
            {
                IdRuta = idRuta,
                OrigenParadaId = origenParadaId,
                DestinoParadaId = destinoParadaId
            }));

        

        // (Opcional) GET tarifa exacta por tramo (útil para calcular precio)
        [HttpGet("tarifa-tramo/exacta")]
        [Authorize]
        public async Task<IActionResult> GetExacta(
            [FromQuery] int idRuta,
            [FromQuery] int origenParadaId,
            [FromQuery] int destinoParadaId) =>
            Ok(await Mediator.Send(new GetTarifaDeTramoActualQuery
            {
                IdRuta = idRuta,
                OrigenParadaId = origenParadaId,
                DestinoParadaId = destinoParadaId
            }));

        // POST crear
        [HttpPost("guardar")]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] CreateTarifaTramoCommand command) =>
            Ok(await Mediator.Send(command));

        // PUT actualizar (por PK simple)
        [HttpPut("tarifa-tramo/{idTarifaTramo:int}")]
        [Authorize]
        public async Task<IActionResult> Put([FromRoute] int idTarifaTramo, [FromBody] UpdateTarifaTramoCommand command)
        {
            if (idTarifaTramo != command.IdTarifaTramo)
                return BadRequest("El id de la ruta no coincide con el payload.");
            return Ok(await Mediator.Send(command));
        }

        // DELETE eliminar (por PK simple)
        [HttpDelete("tarifa-tramo/{idTarifaTramo:int}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int idTarifaTramo) =>
            Ok(await Mediator.Send(new DeleteTarifaTramoCommand { IdTarifaTramo = idTarifaTramo }));
    }
}
