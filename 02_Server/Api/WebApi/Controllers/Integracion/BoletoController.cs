using Aplicacion.Features.Integracion.Commands.BoletoC;
using Aplicacion.Features.Integracion.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.Integracion
{
    [ApiVersion("1.0")]
    [ApiController]
    public class BoletoController : BaseApiController
    {
        [HttpGet("boleto")]
        [Authorize]
        public async Task<IActionResult> Get() =>
            Ok(await Mediator.Send(new GetAllBoletoQuery()));

        [HttpPost("guardar")]
        public async Task<IActionResult> Post([FromBody] CreateBoletoCommand command) =>
            Ok(await Mediator.Send(command));

        [HttpPost("reservar")]
        [Authorize]
        public async Task<IActionResult> Reservar([FromBody] ReservarBoletoCommand command) =>
            Ok(await Mediator.Send(command));

        [HttpPost("confirmar")]
        [Authorize]
        public async Task<IActionResult> Confirmar([FromBody] ConfirmarBoletosCommand command)
        {
            var res = await Mediator.Send(command);
            return Ok(res);
        }

        [HttpPost("reprogramar")]
        [Authorize]
        public async Task<IActionResult> Reprogramar([FromBody] ReprogramarBoletoCommand command) =>
            Ok(await Mediator.Send(command));

        
        [HttpPost("expirar-caducadas")]
        [Authorize]
        public async Task<IActionResult> ExpirarCaducadas([FromQuery] int ttlMin = 10, [FromQuery] int horasAntes = 2) =>
            Ok(await Mediator.Send(new ExpirarReservasVencidasCommand
            {
                ReservaTtlMinutos = ttlMin,
                VentanaVencimientoHorasAntes = horasAntes
            }));

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateBoletoCommand command)
        {
            if (id != command.IdBoleto) return BadRequest();
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id) =>
            Ok(await Mediator.Send(new DeleteBoletoCommand { IdBoleto = id }));

        [HttpGet("{id}/detalle")]
        [Authorize]
        public async Task<IActionResult> GetDetalle(int id) =>
            Ok(await Mediator.Send(new GetBoletoDetalleQuery { IdBoleto = id }));
    }
}
