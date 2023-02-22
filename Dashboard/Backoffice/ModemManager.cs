using Dashboard.Backoffice.Huawei;
using Dashboard.Backoffice.Models;
using Dashboard.Backoffice.Zte;

namespace Dashboard.Backoffice
{
    public static class ModemManager
    {
        public static  Dictionary<int, Modem> Modems { get; set; } = new();
        public static ExternalIpMonitor IpMonitor { get; set; }

        static ModemManager()
        {
            Load();
            IpMonitor = new(modems: Modems);
        }

        static void Load()
        {
            //192.168.2.1, huawei
            Modem m1 = new Modem()
            {
                Host = "192.168.2.1",
                Port = 8002,
                Type = ModemTypeEnum.Huawei,
                Proxy = new Proxy
                {
                    Type = ProxyTypeEnum.http,
                    Login = "viking01",
                    Password = "A000000a",
                    Ip = "100.92.194.11",
                    Port = 8002
                }
            };
            Modems.Add(m1.Port, m1);

            //192.168.3.1, huawei
            Modem m2 = new Modem()
            {
                Host = "192.168.3.1",
                Port = 8003,
                Type = ModemTypeEnum.Huawei,
                Proxy = new Proxy
                {
                    Type = ProxyTypeEnum.http,
                    Login = "viking01",
                    Password = "A000000a",
                    Ip = "100.92.194.11",
                    Port = 8003
                }
            };
            Modems.Add(m2.Port, m2);

            //192.168.9.1, zte
            Modem m3 = new Modem()
            {
                Host = "192.168.9.1",
                Port = 8009,
                Type = ModemTypeEnum.Huawei,
                Proxy = new Proxy
                {
                    Type = ProxyTypeEnum.http,
                    Login = "viking01",
                    Password = "A000000a",
                    Ip = "100.92.194.11",
                    Port = 8009
                }
            };
            Modems.Add(m3.Port, m3);
        }

        public static async Task<bool> RebootModemAsync(int id)
        {
            if (!Modems.ContainsKey(id)) return false;

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

            _ = await ExternalIpMonitor.UpdateExternalIpAsync(modem: Modems[id]);


            return true;
        }
    }
}
