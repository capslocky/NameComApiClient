using System;
using NameComApiClient;

namespace ClientRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            TestAlpha();
            Console.ReadKey();
        }


        private static void TestAlpha()
        {
            var client = new NameComClient();
            var domains = client.GetDomains();

            foreach (var domain in domains)
            {
                Console.WriteLine(domain);
            }

            var dnsRecords = client.GetDnsRecords("on-localhost.com");

            foreach (var record in dnsRecords)
            {
                Console.WriteLine(record);
            }

            DnsRecordCreate newRecord = new DnsRecordCreate();
            newRecord.hostname = "alpha";
            newRecord.type = "A";
            newRecord.content = "127.0.0.7";
            newRecord.ttl = 300;
            newRecord.priority = 10;

            client.CreateDnsRecord("on-localhost.com", newRecord);
        }
    }
}
