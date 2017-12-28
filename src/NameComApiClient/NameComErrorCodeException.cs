using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;

namespace NameComApiClient
{
    public class NameComErrorCodeException : NameComException
    {
        public int ErrorCode { get; }
        public string ResponseMessage { get; }


        public NameComErrorCodeException(int errorCode, string message, IRestClient client, IRestRequest request) : base(client, request, null)
        {
            ErrorCode = errorCode;
            ResponseMessage = message;
        }

        public override string Message => "Code " +  ErrorCode + ": " + ResponseMessage + " => " + base.Message;
    }
}
