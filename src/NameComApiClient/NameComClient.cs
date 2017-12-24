using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using RestSharp;
using RestSharp.Deserializers;

namespace NameComApiClient
{
    public class NameComClient
    {
        private const string ApiUrl = "https://api.name.com/api";
        private const string UserNameKey = "NameComApiUserName";
        private const string TokenKey = "NameComApiToken";

        private readonly RestClient _client;
        

        public NameComClient()
        {
            string userName = GetMachineVarialbe(UserNameKey);
            string token = GetMachineVarialbe(TokenKey);

            _client = new RestClient(ApiUrl);
            _client.AddDefaultHeader("Api-Username", userName);
            _client.AddDefaultHeader("Api-Token", token);

            //for Fiddler
            //needed because of .NET Core bug with proxy
            _client.Proxy = new WebProxy("127.0.0.1", 8888);

            //server returns invalid header
            //Content-Type: text/html; charset=UTF-8
            //instead of
            //Content-Type: application/json; charset=UTF-8
            _client.RemoveHandler("text/html");
            _client.AddHandler("text/html", new JsonDeserializer());
        }



        private string GetMachineVarialbe(string key)
        {
            string value = Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Machine);

            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException("Machine environment variable not set: " + key);
            }

            return value;
        }


        //            RestRequest request = new RestRequest("/hello", Method.GET);

        public string[] GetDomains()
        {
            var request = new RestRequest("/domain/list");
            request.RootElement = "domains";
            var result = _client.Execute<Dictionary<string, object>>(request);

            string[] domains = result.Data.Keys.ToArray();
            return domains;
        }

        public DnsRecord[] GetDnsRecords(string domain)
        {
            var request = new RestRequest("/dns/list/" + domain);
            request.RootElement = "records";
            var response = _client.Execute<List<DnsRecord>>(request);


            var deserializer = new JsonDeserializer();
            deserializer.RootElement = "records";
            var array = deserializer.Deserialize<List<DnsRecord>>(response);

            return response.Data.ToArray();
        }


        public void CreateDnsRecord(string domain, DnsRecordCreate record)
        {
            var request = new RestRequest("/dns/create/" + domain, Method.POST);
            request.AddJsonBody(record);

            var response = _client.Execute(request);
        }


    }

    public class DnsRecord
    {
        public string Content { get; set; }
        public string Create_Date { get; set; }
        public string Name { get; set; }
        public string Record_ID { get; set; }
        public string TTL { get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            return string.Format("{0, -30} => {1}", this.Name, this.Content);
        }
    }


    public class DnsRecordCreate
    {
        public string hostname { get; set; }
        public string type { get; set; }
        public string content { get; set; }
        public int ttl { get; set; }
        public int priority { get; set; }

    }

}
