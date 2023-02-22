using Dashboard.Backoffice.Helpers;
using Dashboard.Backoffice.Models;

namespace Dashboard.Backoffice.Zte
{
    public class ZteWorker : IDisposable
    {
        private HttpClient Client { get; set; }
       private Modem Modem { get; set; }

        public ZteWorker(Modem modem)
        {
            Modem = modem;
            Client = HttpTools.GetHttpClient(proxy: Modem.Proxy, timeout: 3 * 1000);
            HttpClientConfig();
        }

        private void HttpClientConfig()
        {
            Client.DefaultRequestHeaders.Add("User-Agent", @"Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36");
            Client.DefaultRequestHeaders.Add("Referer", $"http://{Modem.Host}/index.html");
            Client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");

        }

        public async Task<bool> ChangeNetworkStateAsync(bool ifConnect = true)
        {
            var task = ifConnect ? "CONNECT_NETWORK" : "DISCONNECT_NETWORK";
            var url = $@"http://{Modem.Host}/goform/goform_set_cmd_process";

            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("isTest", "false"),
                    new KeyValuePair<string, string>("notCallback", "true"),
                    new KeyValuePair<string, string>("goformId", task)
                });
                var result = await Client.PostAsync(url, content);
                string resultContent = await result.Content.ReadAsStringAsync();
                return resultContent.Contains("success");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return false;
        }

        public void Dispose()
        {
            try
            {
                Client.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}

/*
 * http://192.168.9.1/goform/goform_set_cmd_process

данные запроса
isTest=false&notCallback=true&goformId=DISCONNECT_NETWORK

данные запроса
isTest=false&notCallback=true&goformId=CONNECT_NETWORK

Заголовки:
User-Agent:Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36
Referer: http://192.168.9.1/index.html
X-Requested-With: XMLHttpRequest

Ответ:
{"result":"success"}
 *
 */
