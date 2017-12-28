using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;

namespace NameComApiClient
{
    public class NameComException : Exception
    {
        public NameComException(string message) : base(message)
        {
        }

        public NameComException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public NameComException(IRestClient client, IRestRequest request, Exception innerException) 
            : base(GetDescription(client, request), innerException)
        {
        }


        private static string GetDescription(IRestClient client, IRestRequest request)
        {
            return "Request to name.com API failed. URL is " + client.BuildUri(request).AbsoluteUri;
        }

    }
}
