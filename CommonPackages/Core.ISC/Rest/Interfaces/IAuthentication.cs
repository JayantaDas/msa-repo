using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.ISC.Rest
{
    public interface IAuthentication
    {
        Task SetRequestAuthentication(IRestRequest request);
    }
}
