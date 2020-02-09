using Core.Common;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Core.ISC.Rest
{
    public class RestProtocolContextMiddleware
    {
        private readonly RequestDelegate next;

        public RestProtocolContextMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context, IProtocolContext protocolContext)
        {
            foreach (var key in ProtocolConstants.INCOMING_HEADERS)
            {
                if (context?.Request?.Headers?.ContainsKey(key) == true)
                    protocolContext.Headers.AddSeperately(key, context.Request.Headers[key]);
            }

            await this.next?.Invoke(context);
        }
    }
}
