using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomesForAll.Utils.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionManager(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionManager>();
        }
    }
}
