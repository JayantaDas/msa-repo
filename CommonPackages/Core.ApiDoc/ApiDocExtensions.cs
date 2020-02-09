using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ApiDoc
{
    public static class ApiDocExtensions
    {
        public static IApplicationBuilder UseApiDoc(this IApplicationBuilder app, IConfiguration configuration)
        {
            var serviceTitle = configuration["app:serviceTitle"] ?? string.Empty;
            var servicePrefix = configuration["app:servicePrefix"] ?? ApiDocConstants.DEFAULT_APIROUTE_PREFIX;
            //var apiDocRoute = configuration["app:apiDocRoute"] ?? ApiDocConstants.DEFAULT_APIDOC_ROUTE;

            var swaggerRoute = servicePrefix;// + "/" + apiDocRoute;
            var routeTemplate = swaggerRoute + ApiDocConstants.ROUTE_TEMPLATE;
            var swaggerEndPoint = "/" + swaggerRoute + ApiDocConstants.JSON_ENDPOINT;

            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            app.UseSwagger(option => { option.RouteTemplate = routeTemplate; })
               .UseSwaggerUI(c =>
                  {
                      c.SwaggerEndpoint(swaggerEndPoint, serviceTitle);
                      c.RoutePrefix = swaggerRoute;
                  });
               
            return app;
        }
    }
}
