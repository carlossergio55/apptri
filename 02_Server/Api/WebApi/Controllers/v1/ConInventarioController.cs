//using Aplicacion.Features.Composicion.Queries;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Threading.Tasks;
//using Webapi.Controllers.v1;

//namespace WebApi.Controllers.v1
//{
//    [ApiVersion("1.0")]
//    public class ConInventarioController : BaseApiController
//    {

//        [HttpGet("GetConInventario")]
//        [Authorize]
//        public async Task<IActionResult> Get()
//        {
//            return Ok(await Mediator.Send(new GetAllConInventarioQuery()));
//        }
//    }
//}
