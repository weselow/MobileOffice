using System.Diagnostics;
using Dashboard.Backoffice.Helpers;
using Dashboard.Backoffice.Huawei.Models.Dialup;
using Dashboard.Backoffice.Huawei.Models.Net;
using Dashboard.Backoffice.Models;

namespace Dashboard.Backoffice.Huawei
{
    public static class HuaweiManager
    {
        public static async Task<bool> Reboot(Modem modem)
        {
            Stopwatch sw = Stopwatch.StartNew();

            var oldIp = await HttpTools.GetExternalIp(modem.Proxy);
            modem.PrevExternalIp = oldIp;
            modem.AddLogMessage($@"Результат запроса внешнего ip до перезагрузки: {oldIp}.");

            //пробуем первый вариант перезагрузки
            modem.AddLogMessage($@"Запускаем перезагрузку RebootByNetSwitchAsync.");
            var sResult = await RebootByNetSwitchAsync(modem: modem);
            Thread.Sleep(5*1000);
            modem.AddLogMessage($@"Результат перезагрузки RebootByNetSwitchAsync: {sResult}.");

            modem.ExternalIp = await HttpTools.GetExternalIp(modem.Proxy);
            modem.AddLogMessage($@"Результат запроса внешнего ip после перезагрузки: {oldIp}.");
            if (sResult && !string.IsNullOrEmpty(modem.ExternalIp) && modem.ExternalIp != oldIp)
            {
                //если перезагрузка вернула true, и получен новый ip
                //то операция прошла успешно.
                var msg = $"Перезагрузка модема {modem.Port} заняла {sw.ElapsedMilliseconds/1000} сек, oldIp {oldIp}, newIp {modem.ExternalIp}";
                modem.AddLogMessage(msg);
                MessageCenter.Info.Add(msg);
                return true;
            }

            //пробуем второй вариант перезагрузки
            modem.AddLogMessage($@"Запускаем перезагрузку RebootDeviceAsync.");
            var dResult = await RebootDeviceAsync(modem: modem);
            Thread.Sleep(3 * 1000);
            modem.AddLogMessage($@"Результат перезагрузки RebootDeviceAsync: {dResult}.");

            modem.ExternalIp = await HttpTools.GetExternalIp(modem.Proxy);
            modem.AddLogMessage($@"Результат запроса внешнего ip после перезагрузки: {oldIp}.");
            if (dResult && !string.IsNullOrEmpty(modem.ExternalIp) && modem.ExternalIp != oldIp)
            {
                //если перезагрузка вернула true, и получен новый ip
                //то операция прошла успешно.
                var msg = $"Перезагрузка модема {modem.Port} заняла {sw.ElapsedMilliseconds/1000} сек, oldIp {oldIp}, newIp {modem.ExternalIp}";
                modem.AddLogMessage(msg);
                MessageCenter.Info.Add(msg);
                return true;
            }

            //в остальных случаях возвращем false
            var mc = $"Перезагрузка c ошибкой, заняла {sw.ElapsedMilliseconds/1000} сек, oldIp {oldIp}, newIp {modem.ExternalIp}";
            modem.AddLogMessage(mc);
            MessageCenter.Errors.Add(mc);
            return false;
        }

        /// <summary>
        /// Перезагрузка путем переключения в режим 3G и обратно.
        /// </summary>
        /// <param name="modem"></param>
        /// <returns></returns>
        private static async Task<bool> RebootByNetSwitchAsync(Modem modem)
        {
            HuaweiWorker dobby = new(modem: modem);

            //получаем токен
            var result = await dobby.GetFirstTokenAsync();
            if (!result)
            {
                Console.WriteLine("Базовая ошибка при запросе токена - или ошибка, или модем не huawei.");
                
                return false;
            }

            // POST Отключаем передачу данных
            result = await dobby.MobileDataSwitchAsync(MobileDataSwitchEnum.Disable);
            if (!result)
            {
                Console.WriteLine("Базовая ошибка на шаге - Отключаем передачу данных.");
                return false;
            }

            //ожидаем пока отключится
            Console.Write("Ожидаем отключения передачи данных ");
            for (int i = 0; i < 60; i++)
            {
                var t = await dobby.MobileDataSwitchGetInfoAsync();
                if (t == null)
                {
                    Console.Write(".");
                    Thread.Sleep(1000);
                    continue;
                }
                if (t.Dataswitch == "0") break;
            }
            Console.WriteLine();

            // POST Переключение на 3g
            result = await dobby.ChangeNetModeAsync(networkMode: NetworkMode.Only3G, networkBand: NetworkBand.AllSupported, lteBand: LteBand.AllBands);

            //ожидаем пока переключится
            Console.Write("Ожидаем переключение на 3g ");
            for (int i = 0; i < 60; i++)
            {
                var t = await dobby.NetModeStatusAsync();
                if (t == null)
                {
                    Console.Write(".");
                    Thread.Sleep(1000);
                    continue;
                }
                if (t.NetworkMode == "99") Console.WriteLine("NetworkMode вернулся 99, то есть, без изменений.");
                if (t.NetworkMode == "02") break;
            }
            Console.WriteLine();

            // POST Переключение на 4g-3g lte-UMTS
            result = await dobby.ChangeNetModeAsync(networkMode: NetworkMode.Only4G + NetworkMode.Only3G, networkBand: NetworkBand.AllSupported, lteBand: LteBand.AllBands);

            //ожидаем пока переключится
            Console.Write("Ожидаем переключение на 4g ");
            for (int i = 0; i < 60; i++)
            {
                var t = await dobby.NetModeStatusAsync();
                if (t == null)
                {
                    Console.Write(".");
                    Thread.Sleep(1000);
                    continue;
                }
                if (t.NetworkMode == "99") Console.WriteLine("NetworkMode вернулся 99, то есть, без изменений.");
                if (t.NetworkMode == "0302") break;
            }
            Console.WriteLine();

            // POST Включаем передачу данных
            result = await dobby.MobileDataSwitchAsync(MobileDataSwitchEnum.Enable);
            if (!result)
            {
                Console.WriteLine("Базовая ошибка на шаге - Включаем передачу данных.");
                return false;
            }

            //ожидаем пока отключится
            Console.Write("Ожидаем включения передачи данных ");
            for (int i = 0; i < 60; i++)
            {
                var t = await dobby.MobileDataSwitchGetInfoAsync();
                if (t == null)
                {
                    Console.Write(".");
                    Thread.Sleep(1000);
                    continue;
                }
                if (t.Dataswitch == "1") break;
            }
            Console.WriteLine();
            dobby.Dispose();

            return true;
        }

        /// <summary>
        /// Перезагрузка путем ребута устройства.
        /// </summary>
        /// <param name="modem"></param>
        /// <returns></returns>
        private static async Task<bool> RebootDeviceAsync(Modem modem)
        {
            HuaweiWorker dobby = new(modem: modem);

            //получаем токен
            var result = await dobby.GetFirstTokenAsync();
            if (!result)
            {
                Console.WriteLine("Базовая ошибка при запросе токена - или ошибка, или модем не huawei.");
                return false;
            }

            //отправляем запрос на перезагрузку
            result = await dobby.RebootDeviceAsync();
            if (!result)
            {
                Console.WriteLine("Ошибка при отправке запроса на перезагрузку модема.");                 
                return false;
            }
            Thread.Sleep(10*1000);

            //ожидаем пока снова не покажется в сети
            result = false;
            Console.WriteLine("Ожидаем перезагрузку модема ");
            for (int i=0; i < 60; i++)
            {
                result = await dobby.GetFirstTokenAsync();
                if (result) break;
                Console.Write(".");
                Thread.Sleep(1000);
            }
            Console.WriteLine();
            dobby.Dispose();

            return result;
        }
    }
}
