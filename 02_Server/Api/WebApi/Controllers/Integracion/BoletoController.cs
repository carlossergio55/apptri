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
        [Authorize]
        public async Task<IActionResult> Post(CreateBoletoCommand command) =>
            Ok(await Mediator.Send(command));

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateBoletoCommand command)
        {
            if (id != command.IdBoleto) return BadRequest();
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id) =>
            Ok(await Mediator.Send(new DeleteBoletoCommand { IdBoleto = id }));
    }
}
