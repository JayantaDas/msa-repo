using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ISC.Rest
{
    public static class RestProtocolContextExtensions
    {
        public static void UseRestProtocolContext(this IApplicationBuilder app)
        {
            app.UseMiddleware<RestProtocolContextMiddleware>();
        }
    }
}
