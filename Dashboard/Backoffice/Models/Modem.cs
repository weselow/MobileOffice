using System.ComponentModel.DataAnnotations;

namespace Dashboard.Backoffice.Models
{
    public class Modem
    {
        [Display(Name="Ip адрес модема (host)")]
        public string Host { get; set; } = string.Empty;
        [Display(Name = "Порт модема (порт)")]
        public int Port { get; set; }
        [Display(Name = "Производитель модема")]
        public ModemTypeEnum Type { get; set; }
        public string ExternalIp { get; set; } = string.Empty;
        public Proxy Proxy { get; set; } = new();
        public Dictionary<DateTime, string> Logs { get; set; } = new();
        private int LogsLimit { get; set; } = 100;
        private readonly object _lock = new();
        public void AddLogMessage(string text)
        {
            lock (_lock)
            {
                var dt = DateTime.Now;
                if (Logs.ContainsKey(dt))
                {
                    Logs[dt] += Environment.NewLine + text;
                }
                else
                {
                    Logs.Add(dt, text);
                }

                if (Logs.Keys.Count <= LogsLimit) return;
                var k = Logs.Keys.Count - LogsLimit;
                for (var i = 0; i < k; i++)
                {
                    Logs.Remove(Logs.Keys.MinBy(t => t));
                }
            }


        }

        public bool CopyFrom(Modem newData)
        {
            this.Host = newData.Host;
            this.Port = newData.Port;
            this.Type = newData.Type;
            this.ExternalIp = newData.ExternalIp;
            this.Proxy = newData.Proxy;
            return true;
        }
    }
}
