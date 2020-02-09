using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Common
{
    public static class ProtocolContextExtensions
    {
        public static void AddProtocolContext(this IServiceCollection services)
        {
            services.AddScoped<IProtocolContext, ProtocolContext>();
        }
    }
}
