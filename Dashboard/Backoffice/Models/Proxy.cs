using System.ComponentModel.DataAnnotations;

namespace Dashboard.Backoffice.Models
{
    public class Proxy
    {
        [Display(Name="Тип прокси")]
        public ProxyTypeEnum Type { get; set; }
        [Display(Name = "Login")]
        public string Login { get; set; } = string.Empty;
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;
        [Display(Name = "Ip")]
        public string Ip { get; set; } = string.Empty;
        [Display(Name = "Port")]
        public int Port { get; set; }
    }
}
