using Aplicacion.DTOs.Integracion;                       // ParadaDto
using Aplicacion.Features.Integracion.Commands.ViajeC;   // Create/Update/Delete/Generar/Actualizar estados
using Aplicacion.Features.Integracion.Queries;           // GetAllViajeQuery, GetParadasDeViajeQuery, SeatmapPorTramoQuery
using Aplicacion.Wrappers;                               // Response<T>
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.Integracion
{
    [ApiVersion("1.0")]
    [ApiController]
    public class ViajeController : BaseApiController
    {
        [HttpGet("viaje")]
        [Authorize]
        public async Task<IActionResult> Get() =>
            Ok(await Mediator.Send(new GetAllViajeQuery()));

        [HttpPost("guardar")]
        [Authorize]
        public async Task<IActionResult> Post(CreateViajeCommand command) =>
            Ok(await Mediator.Send(command));

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateViajeCommand command)
        {
            if (id != command.IdViaje) return BadRequest();
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id) =>
            Ok(await Mediator.Send(new DeleteViajeCommand { IdViaje = id }));

        [HttpPost("generar-proximos")]
        [Authorize]
        public async Task<IActionResult> GenerarProximos([FromBody] GenerarViajesProximosCommand cmd) =>
            Ok(await Mediator.Send(cmd));

        [HttpPost("actualizar-estados")]
        [Authorize]
        public async Task<IActionResult> ActualizarEstados([FromBody] ActualizarEstadosViajeCommand cmd) =>
            Ok(await Mediator.Send(cmd));

        // ===== Paradas del viaje (devuelve Response<List<ParadaDto>>) =====
        [HttpGet("{id}/paradas")]
        [Authorize]
        public async Task<IActionResult> GetParadas(int id)
        {
            // Esta query te está devolviendo List<ParadaVm>
            var vms = await Mediator.Send(new GetParadasDeViajeQuery { IdViaje = id });

            // Mapear manualmente ParadaVm -> ParadaDto (los campos que uses en front)
            var dtos = vms.Select(p => new ParadaDto
            {
                IdParada = p.IdParada,
                Nombre = p.Nombre
                // si ParadaDto tiene más campos y existen en el VM, mapéalos aquí
            }).ToList();

            return Ok(new Response<List<ParadaDto>>(dtos));
        }

        // ===== Seatmap por tramo (Response<List<SeatmapSeatDto>>) =====
        [HttpGet("{id}/seatmap")]
        [Authorize]
        public async Task<IActionResult> GetSeatmap(
            int id,
            [FromQuery] int origenId,
            [FromQuery] int destinoId,
            [FromQuery] int reservaTtlMinutos = 10)
        {
            var resp = await Mediator.Send(new SeatmapPorTramoQuery
            {
                ViajeId = id,
                OrigenParadaId = origenId,
                DestinoParadaId = destinoId,
                ReservaTtlMinutos = reservaTtlMinutos
            });

            return Ok(resp); // ya es Response<List<SeatmapSeatDto>>
        }
    }
}
