using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ISC.Rest
{
    public class Http
    {
        
        public enum Method
        {
            DELETE,
            GET,
            HEAD,
            OPTIONS,
            POST,
            PUT,
            TRACE
        }

        public enum AuthenticationMethod
        {
            Basic,
        }
    }
}
