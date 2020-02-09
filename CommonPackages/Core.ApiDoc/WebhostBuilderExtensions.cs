using Microsoft.AspNetCore.Hosting;
using System;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;

namespace Core.ApiDoc
{
    public static class WebhostBuilderExtensions
    {
        public static IWebHostBuilder UseApiDoc(this IWebHostBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ConfigureServices((context, collection) =>
            {
                var serviceTitle = context.Configuration["app:serviceTitle"] ?? string.Empty;
                var serviceVersion = context.Configuration["app:serviceVersion"] ?? "v1";

                collection.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc(serviceVersion, new OpenApiInfo { Title = serviceTitle, Version = serviceVersion });
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Please enter token with Bearer into field e.g.  bearer {token}",
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey
                    });
                    c.AddSecurityRequirement(new OpenApiSecurityRequirement());
                });
            });

            return builder;
        }
    }
}
