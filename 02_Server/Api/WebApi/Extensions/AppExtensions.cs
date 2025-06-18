using Microsoft.AspNetCore.Builder;
using Webapi.Middlewares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webapi.Extensions
{
    public static class AppExtensions
    {

        public static void UseErrorHandlingMiddleware(this IApplicationBuilder app) {

            app.UseMiddleware<ErrorHandlerMiddleware>();

        }


    }
}
