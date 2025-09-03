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
        // Obtener todas las asociaciones Ruta–Parada
        //[HttpGet("ruta-parada")]
        //public async Task<IActionResult> Getw()
        //{
        //    return Ok(await Mediator.Send(new GetAllRutaParadaQuery()));
        //}

        // Obtener todas las paradas de una ruta específica
        [HttpGet("GetAllRutaParadaFull")]
        public async Task<IActionResult> GetAllRutaParadaFull()
    => Ok(await Mediator.Send(new GetAllRutaParadaFullQuery()));

        [HttpGet("GetAllIdRutaParada")]
        public async Task<IActionResult> GetAllRutaParada([FromQuery] int? idRuta)
            => Ok(await Mediator.Send(new GetAllRutaParadaQuery(idRuta)));

        // Crear nueva relación Ruta–Parada
        [HttpPost("guardar")]
        [Authorize]
        public async Task<IActionResult> Post(CreateRutaParadaCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        // Actualizar relación Ruta–Parada
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateRutaParadaCommand command)
        {
            if (id != command.IdRutaParada)
                return BadRequest("El ID no coincide con el payload.");
            return Ok(await Mediator.Send(command));
        }

        // Eliminar relación Ruta–Parada
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteRutaParadaCommand { IdRutaParada = id }));
        }
    }
}
