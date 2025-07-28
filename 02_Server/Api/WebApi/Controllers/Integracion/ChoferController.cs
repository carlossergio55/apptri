﻿using Aplicacion.Features.Integracion.Commands;

using Aplicacion.Features.Integracion.Commands.ChoferC;
using Aplicacion.Features.Integracion.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Webapi.Controllers.v1;

namespace WebApi.Controllers.Integracion
{
    [ApiVersion("1.0")]
    [ApiController]
    public class ChoferController : BaseApiController
    {

        [HttpGet("chofer")]
        [AllowAnonymous] // 👈 Esto permite el acceso sin autenticación
        public async Task<IActionResult> Get()
        {
            return Ok(await Mediator.Send(new GetAllChoferQuery()));
        }

        [HttpPost("Guardar")]
        [AllowAnonymous]
        public async Task<IActionResult> Post(CreateChoferCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteChoferCommand { IdChofer = id }));
        }
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Put(int id, UpdateChoferCommand command)
        {
            if (id != command.IdChofer)
            {
                return BadRequest();
            }
            return Ok(await Mediator.Send(command));
        }


    }
}
