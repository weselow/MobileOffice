namespace Dashboard.Backoffice.Models
{
    public class Proxy
    {
        public ProxyTypeEnum Type { get; set; }
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Ip { get; set; } = string.Empty;
        public int Port { get; set; }
    }
}
