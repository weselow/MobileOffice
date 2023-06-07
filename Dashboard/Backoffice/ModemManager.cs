using System.Collections.Concurrent;
using System.Timers;
using Dashboard.Backoffice.Huawei;
using Dashboard.Backoffice.Models;
using Dashboard.Backoffice.Zte;
using Timer = System.Timers.Timer;

namespace Dashboard.Backoffice
{
    public static class ModemManager
    {
        public static Dictionary<int, Modem> Modems { get; set; }
        public static ExternalIpMonitor IpMonitor { get; set; }
        private static ConcurrentDictionary<int, Modem> ModemInProgress { get; set; }
        public static SettingsMonitor SettingsMonitor { get; set; }

        private static Timer Atimer { get; set; }

        static ModemManager()
        {
            Modems = new();
            ModemInProgress = new();
            SettingsMonitor = new(modems: Modems);
            SettingsMonitor.LoadSettings();
            IpMonitor = new(modems: Modems);

            //запускаем таймер раз в минуту
            Atimer = new(60 * 1000) { AutoReset = true, Enabled = true };
            Atimer.Elapsed += OnTimerEvent;
        }

        private static void OnTimerEvent(object? sender, ElapsedEventArgs e)
        {
            foreach (var m in Modems
                         .Where(t => t.Value.IfTimerRebootAllowed
                                   & (DateTime.Now - t.Value.LastRebootedTime).TotalMinutes > t.Value.RebootDelay))
            {
                _ = RebootModemAsync(m.Key);
            }
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

            Modems[id].LastRebootedTime = DateTime.Now;
            ModemInProgress.TryRemove(id, out _);

            return true;
        }

        public static async Task<bool> DeleteModemAsync(int id)
        {
            await Task.Run(() => true);
            if (!Modems.ContainsKey(id)) return false;
            while (ModemInProgress.Keys.Contains(id)) Thread.Sleep(500);
            while (Modems.ContainsKey(id) && !Modems.Remove(id))
            {
                MessageCenter.Info.Add($"Не смогли удалить модем {id}.");
                Thread.Sleep(500);
            }
            MessageCenter.Warnings.Add($"Успешно удален модем {id}.");
            return true;
        }
    }
}
