using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.ISC.Rest
{
    internal interface IRestBuilder
    {
        /// <summary>
        /// Adds a global querystring parameter. If a parameter with the same name exists in the request it will override the global one.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void AddParameter(string name, string value);

        /// <summary>
        /// Adds Header to the HTTP request. If a header with the same name exists in the request it will override the global one.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void AddHeader(string name, string value);

        #region ExecuteAsync
        /// <summary>
        /// Executes a specified request asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        Task<IRestResponse<string>> ExecuteAsync(IRestRequest request);

        /// <summary>
        /// Executes a specified request asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        Task<IRestResponse<T>> ExecuteAsync<T>(IRestRequest request);

        #endregion
    }
}
