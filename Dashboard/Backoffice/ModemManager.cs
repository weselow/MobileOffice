using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using System.Text.Json;
using Dashboard.Backoffice.Huawei;
using Dashboard.Backoffice.Models;
using Dashboard.Backoffice.Zte;

namespace Dashboard.Backoffice
{
    public static class ModemManager
    {
        public static Dictionary<int, Modem> Modems { get; set; }
        public static ExternalIpMonitor IpMonitor { get; set; }
        private static ConcurrentDictionary<int, Modem> ModemInProgress { get; set; }
        public static SettingsMonitor SettingsMonitor { get; set; }

        static ModemManager()
        {
            Modems = new();
            ModemInProgress = new();
            SettingsMonitor = new(modems: Modems);
            SettingsMonitor.LoadSettings();
            IpMonitor = new(modems: Modems);
        }
        public static async Task<bool> RebootModemAsync(int id)
        {
            if (!Modems.ContainsKey(id)) return false;

            //исключаем возможность одновременной перезагрузки одного и того же модема
            while (ModemInProgress.Keys.Contains(id)) Thread.Sleep(500);
            while (ModemInProgress.TryAdd(id, Modems[id])) Thread.Sleep(500);

            switch (Modems[id].Type)
            {
                case ModemTypeEnum.Huawei:
                    _ = await HuaweiManager.Reboot(modem: Modems[id]);
                    break;

                case ModemTypeEnum.Zte:
                    _ = await ZteManager.Reboot(modem: Modems[id]);
                    break;

                default:
                    break;
            }

            ModemInProgress.TryRemove(id, out _);

            return true;
        }
    }
}
