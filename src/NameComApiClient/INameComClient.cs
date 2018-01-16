using System;

namespace NameComApiClient
{
    public interface INameComClient
    {
        string[] GetDomains();
        DnsRecord[] GetDnsRecords(string domain);
        DnsRecord CreateDnsRecord(string domain, DnsRecordCreate record);
        void DeleteDnsRecord(string domain, long recordId);
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
