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
        private readonly IRestClient _client;

        public NameComClient(IRestClient client, string userName, string token)
        {
            _client = client;
            _client.AddDefaultHeader("Api-Username", userName);
            _client.AddDefaultHeader("Api-Token", token);

            //server returns invalid header for json content
            //Content-Type: text/html; charset=UTF-8
            //instead of
            //Content-Type: application/json; charset=UTF-8
            _client.RemoveHandler("text/html");
            _client.AddHandler("text/html", new JsonDeserializer());
        }


        //            RestRequest request = new RestRequest("/hello", Method.GET);

        public string[] GetDomains(){
            var request = new RestRequest("/api/domain/list");
            var response = ExecuteRequest(request);

            var deserializer = new JsonDeserializer { RootElement = "domains" };
            var dic = deserializer.Deserialize<Dictionary<string, object>>(response);
            return dic.Keys.ToArray();
        }

        public DnsRecord[] GetDnsRecords(string domain){
            var request = new RestRequest("/api/dns/list/" + domain);
            var response = ExecuteRequest(request);

            var deserializer = new JsonDeserializer {RootElement = "records"};
            var list = deserializer.Deserialize<List<DnsRecord>>(response);
            return list.ToArray();
        }


        public DnsRecord CreateDnsRecord(string domain, DnsRecordCreate record)
        {
            var request = new RestRequest("/api/dns/create/" + domain, Method.POST);
            request.AddJsonBody(record);
            var response = ExecuteRequest(request);

            var deserializer = new JsonDeserializer();
            var newRecord = deserializer.Deserialize<DnsRecord>(response);
            return newRecord;
        }


        public void DeleteDnsRecord(string domain, long recordId)
        {
            var request = new RestRequest("/api/dns/delete/" + domain, Method.POST);
            request.AddJsonBody(new {record_id = recordId});
            var response = ExecuteRequest(request);
        }

        private const long CodeOK = 100;

        private IRestResponse ExecuteRequest(IRestRequest request)
        {
            try
            {
                var response = _client.Execute<Dictionary<string, object>>(request);

                if (!response.IsSuccessful)
                {
                    throw new NameComException(_client, request, response.ErrorException);
                }

                var resultDic = response.Data.AsDictionary()["result"].AsDictionary();

                int code = Convert.ToInt32(resultDic["code"]);
                string message = Convert.ToString(resultDic["message"]);

                if (code != CodeOK)
                {
                    throw new NameComErrorCodeException(code, message, _client, request);
                }

                return response;
            }
            catch (NameComException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new NameComException(_client, request, ex);
            }
        }
    }


    internal static class ExtensionMethods
    {
        internal static Dictionary<string, object> AsDictionary(this object obj)
        {
            return (Dictionary<string, object>)obj;
        }
    }

    public class DnsRecord
    {
        public string Content { get; set; }
        public DateTime Create_Date { get; set; }
        public string Name { get; set; }
        public long Record_ID { get; set; }
        public int TTL { get; set; }
        public string Type { get; set; }
        public int? Priority { get; set; }

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
