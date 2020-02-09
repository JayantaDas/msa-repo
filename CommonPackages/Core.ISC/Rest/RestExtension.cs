using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using namespaceRest = Core.ISC.Rest;




namespace Core.ISC
{
    public static class RestExtensions
    {
        public static IServiceCollection AddISC(this IServiceCollection serviceCollection, IConfiguration configuration)
        {

            serviceCollection.AddHttpClient(Options.DefaultName).ConfigurePrimaryHttpMessageHandler(config => new namespaceRest.HttpRedirectClientHandler
            {
                InnerHandler = new HttpClientHandler()
                {
                    AllowAutoRedirect = false
                }
            });
            //serviceCollection.AddHttpClient();
            serviceCollection.AddTransient<namespaceRest.IRest, namespaceRest.Rest>();
            return serviceCollection;
        }
    }
}
