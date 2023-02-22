using Dashboard.Backoffice;
using Dashboard.Backoffice.Models;

namespace Dashboard.Models
{
    public class MainPageModel
    {
        public List<string> Warnings { get; set; } = new();
        public List<string> Errors { get; set; } = new();
        public List<string> Info { get; set; } = new();

        public List<Modem> Modems { get; set; }

        public MainPageModel()
        {
            Modems = ModemManager.Modems.Values.OrderBy(t => t.Host).ToList();
        }

    }
}
