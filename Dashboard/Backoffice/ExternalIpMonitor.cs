using System.Net;
using System.Threading;
using System.Timers;
using Dashboard.Backoffice.Helpers;
using Dashboard.Backoffice.Models;

namespace Dashboard.Backoffice
{
    public class ExternalIpMonitor
    {
        private Dictionary<int, Modem> Modems { get; set; }
        private System.Timers.Timer Atimer { get; set; }
       

        public ExternalIpMonitor(Dictionary<int, Modem> modems)
        {
            Modems = modems;
            Atimer = new System.Timers.Timer( 5 * 60 * 1000) { AutoReset = true, Enabled = true };
            Atimer.Elapsed += UpdateAllExternalIps;
            UpdateAllExternalIps();
        }

        public async void UpdateAllExternalIps(Object? source = null, ElapsedEventArgs? e = null)
        {
            //делаем запрос внешнего айпишника для каждого модема
            foreach (var modem in Modems.Values)
            {
                modem.ExternalIp = await HttpTools.GetExternalIp(proxy: modem.Proxy);
            }
        }

        public static async Task<bool> UpdateExternalIpAsync(Modem modem)
        {
            modem.ExternalIp = await HttpTools.GetExternalIp(proxy: modem.Proxy);
            return !string.IsNullOrEmpty(modem.ExternalIp);
        }
    }
}
