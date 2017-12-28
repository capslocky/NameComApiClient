using System;
using System.IO;
using System.Net;
using Microsoft.Extensions.Configuration;
using NameComApiClient;
using RestSharp;

namespace ClientRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Begin.");
            TestAlpha();
            Console.ReadKey();
        }


        private static void TestAlpha()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("ClientRunnerSettings.json")
                .Build();

            var settings = new Settings();
            config.Bind(settings);

            Console.WriteLine("Settings URL is " +  settings.Url);

            IRestClient restClient = new RestClient(settings.Url);

            //for Fiddler
            restClient.Proxy = new WebProxy("127.0.0.1", 8888);

            var client = new NameComClient(restClient, settings.UserName, settings.Token);
            var domains = client.GetDomains();

            foreach (var domain in domains)
            {
                Console.WriteLine(domain);
            }

            var dnsRecords = client.GetDnsRecords("baur.im");

            foreach (var record in dnsRecords)
            {
                Console.WriteLine(record);
            }

            DnsRecordCreate recordCreate = new DnsRecordCreate();
            recordCreate.hostname = "gamma";
            recordCreate.type = "A";
            recordCreate.content = "127.0.0.9";
            recordCreate.ttl = 300;
            recordCreate.priority = 10;

//            var newRecord = client.CreateDnsRecord("baur.im", recordCreate);

//            client.DeleteDnsRecord("baur.im", 318485350);
        }
    }
}
