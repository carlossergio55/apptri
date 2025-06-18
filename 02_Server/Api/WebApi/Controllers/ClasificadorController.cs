using Aplicacion.Features.Aplicacion.Clasificador.Queries;
using Aplicacion.Features.Clasificador;
using Aplicacion.Features.Clasificador.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    public class ClasificadorController : BaseApiController
    {

   
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(CreateGenClasificadorCommand command)
        {
            return Ok(await Mediator.Send(command));
        }


        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateGenClasificadorCommand command)
        {
            if (id != command.IdgenClasificador)
            {
                return BadRequest();
            }
            return Ok(await Mediator.Send(command));
        }


        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteGenClasificadorCommand { IdgenClasificador = id }));
        }




        [HttpGet("Clasificador")]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            return Ok(await Mediator.Send(new GetAllClasificadorQuery()));
        }

        [HttpGet("GetAllGenClasificadortipo")]
        [Authorize]
        public async Task<IActionResult> GetAllGenClasificadortipo()
        {
            return Ok(await Mediator.Send(new GetAllGenClasificadortipoQuery()));
        }


    }
}
