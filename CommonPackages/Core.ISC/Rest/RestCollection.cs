using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Core.ISC.Rest
{
    public class RestCollection : Dictionary<string, string>
    {

        private readonly CollectionType collectionType = CollectionType.None;
        public enum CollectionType
        {
            None,
            Header,
            QueryStringParameter
        }

        /// <summary>
        /// ContentType of the HTTP request. The default content type is text/plain
        /// </summary>
        public string ContentType { get; private set; } = HttpConstants.DefaultContentType;

        /// <summary>
        /// authentication header 
        /// </summary>
        public string AuthorizationHeader { get; private set; }

        public RestCollection()
        {

        }

        public RestCollection(CollectionType collectionType)
        {
            this.collectionType = collectionType;
        }

        public void AddOrModify(string key, string value)
        {
            if (collectionType == CollectionType.QueryStringParameter)
            {
                key = WebUtility.UrlEncode(key);
                value = WebUtility.UrlEncode(value);
            }

            if (collectionType == CollectionType.Header)
            {
                string lowercaseName = key.ToLower();
                if (lowercaseName == HttpConstants.ContentTypeHeader.ToLower())
                {
                    ContentType = value;
                }
                else if (lowercaseName == HttpConstants.AuthorizationHeader.ToLower())
                {
                    AuthorizationHeader = key;
                }
            }

            if (ContainsKey(key))
            {
                this[key] = value;
            }
            else
            {
                Add(key, value);
            }
        }

        public void MergeWith(RestCollection mergee)
        {
            foreach (var item in mergee)
            {
                AddOrModify(item.Key, item.Value);
            }
        }
    }
}
