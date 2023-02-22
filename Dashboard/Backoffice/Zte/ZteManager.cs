using System.Diagnostics;
using Dashboard.Backoffice.Helpers;
using Dashboard.Backoffice.Models;

namespace Dashboard.Backoffice.Zte
{
    public static class ZteManager
    {
        public static async Task<bool> Reboot(Modem modem)
        {
            Stopwatch sw = Stopwatch.StartNew();

            var oldIp = await HttpTools.GetExternalIp(modem.Proxy);
            modem.AddLogMessage($@"Результат запроса внешнего ip до перезагрузки: {oldIp}.");

            //пробуем первый вариант перезагрузки
            modem.AddLogMessage($@"Запускаем перезагрузку RebootByNetworkDisconnectAsync.");
            var sResult = await RebootByNetworkDisconnectAsync(modem: modem);
            Thread.Sleep(3 * 1000);
            modem.AddLogMessage($@"Результат перезагрузки RebootByNetworkDisconnectAsync: {sResult}.");

            modem.ExternalIp = await HttpTools.GetExternalIp(modem.Proxy);
            modem.AddLogMessage($@"Результат запроса внешнего ip после перезагрузки: {oldIp}.");

            sw.Stop();
            modem.AddLogMessage($"Перезагрузка заняла {sw.ElapsedMilliseconds} мс., oldIp {oldIp}, newIp {modem.ExternalIp}");

            if (sResult && !string.IsNullOrEmpty(modem.ExternalIp) && modem.ExternalIp != oldIp)
            {
                //если перезагрузка вернула true, и получен новый ip
                //то операция прошла успешно.
                return true;
            }
            
            //в остальных случаях возвращем false
            return false;
        }

        /// <summary>
        /// Перезагружаем путем отсоединения от сети.
        /// </summary>
        /// <param name="modem"></param>
        /// <returns></returns>
        public static async Task<bool> RebootByNetworkDisconnectAsync(Modem modem)
        {
           var dobby = new ZteWorker(modem);

            //отключаем сеть
            _ = await dobby.ChangeNetworkStateAsync(ifConnect: false);

            Thread.Sleep(5* 1000);

            //включаем сеть
            _ = await dobby.ChangeNetworkStateAsync(ifConnect: true);

            dobby.Dispose();

            return true;
        }
    }
}
