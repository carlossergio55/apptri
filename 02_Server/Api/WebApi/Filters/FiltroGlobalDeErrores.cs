using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace WebApi.Filters
{
    public class FiltroGlobalDeErrores : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var respuesta = new
            {
                Satisfactorio = false,
                Mensaje = context.Exception.InnerException?.Message ?? context.Exception.Message,
                Errores = (object?)null,
                Datos = (object?)null
            };

            context.Result = new JsonResult(respuesta)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            context.ExceptionHandled = true;
        }
    }
}
