using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Core.ISC.Rest
{
    public interface IRestRequest
    {
        string Path { get; }
        string InnerProperty { get; }
        bool RequiresAuthentication { get; set; }
        HttpMethod Method { get; }
        HttpContent Content { get; }
        RestCollection Parameters { get; }
        RestCollection Headers { get; }

        void AddUrlSegment(string identifier, string value);
        void AddParameter(string name, string value);
        void AddHeader(string name, string value);
        void SetContent(string content);
        void SetContent(object content, string contentType);
        void SetContent(StringContent stringContent);
        void SetContent(FormUrlEncodedContent formUrlEncodedContent);
        void SetAuthentication(Http.AuthenticationMethod authenticationMethod, string token);
    }
}
