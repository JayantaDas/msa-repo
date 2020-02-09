using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.ISC.Rest
{
    public interface IRest
    {
        Uri BaseUrl { get; }
        void SetBaseUrl(string url);
        long AvgRequestTimeMs { get; }

        /// <summary>
        /// Get or Set an authenticator to easily implement authentication with an API
        /// </summary>
        /// <value>
        /// The authenticator instance
        /// </value>
        IAuthentication Authentication { get; set; }

        #region GET
        Task<IRestResponse<string>> GetAsync(string path, bool requiresAuthentication);

        Task<IRestResponse<TResponse>> GetAsync<TResponse>(string path, bool requiresAuthentication);

        #endregion

        #region PUT
        Task<IRestResponse<string>> PutAsync<TBody>(string path, TBody body, bool requiresAuthentication);

        Task<IRestResponse<TResponse>> PutAsync<TResponse, TBody>(string path, TBody body, bool requiresAuthentication);

        #endregion

        #region POST
        Task<IRestResponse<string>> PostAsync<TBody>(string path, TBody body, bool requiresAuthentication);

        Task<IRestResponse<TResponse>> PostAsync<TResponse, TBody>(string path, TBody body, bool requiresAuthentication);

        #endregion

        #region DELETE
        Task<IRestResponse<string>> DeleteAsync<TBody>(string path, TBody body, bool requiresAuthentication);

        Task<IRestResponse<TResponse>> DeleteAsync<TResponse, TBody>(string path, TBody body, bool requiresAuthentication);

        #endregion


    }
}
