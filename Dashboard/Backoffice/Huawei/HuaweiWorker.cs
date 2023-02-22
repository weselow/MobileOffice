using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using Dashboard.Backoffice.Helpers;
using Dashboard.Backoffice.Huawei.Models.Device;
using Dashboard.Backoffice.Huawei.Models.Dialup;
using Dashboard.Backoffice.Huawei.Models.Errors;
using Dashboard.Backoffice.Huawei.Models.Net;
using Dashboard.Backoffice.Huawei.Models.Webserver;
using Dashboard.Backoffice.Models;

namespace Dashboard.Backoffice.Huawei
{
    public class HuaweiWorker : IDisposable
    {
        const string CsrfTokenName = "__RequestVerificationToken";
        public SesTokInfoResponse SessionInfo { get; set; } = new();
        public ApiErrors LastError { get; set; } = new();
        public Exception? LastException { get; set; } = null;
        private HttpClient Client { get; set; }
        private  Modem Modem { get; set; }
        public HuaweiWorker(Modem modem)
        {
            Modem = modem;
            Client = HttpTools.GetHttpClient(proxy: Modem.Proxy, timeout: 1000);
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
        }

        /// <summary>
        /// Запросить токен и куку
        /// </summary>
        /// <returns></returns>
        public async Task<bool> GetFirstTokenAsync()
        {
            var url = $"http://{Modem.Host}/api/webserver/SesTokInfo";
            var response = string.Empty;
            //http 
            try
            {
                response = await Client.GetStringAsync(url);
            }
            catch (TaskCanceledException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e) when (CheckIfSocketException(e))
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            //xml
            if (!string.IsNullOrEmpty(response))
            {
                if (!response.Contains("error"))
                {
                    try
                    {
                        SesTokInfoResponse result = response.DeserializeXml<SesTokInfoResponse>();
                        SessionInfo = result;
                        Console.WriteLine("Успешно получен токен от модема.");
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    try
                    {
                        ApiErrors result = response.DeserializeXml<ApiErrors>();
                        Console.WriteLine($"Ошибка: {result.Code} {result.Message}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }

            return false;
        }

        
        /// <summary>
        /// Отключаем или включаем передачу данных.
        /// </summary>
        /// <param name="switchEnum"></param>
        /// <returns></returns>
        public async Task<bool> MobileDataSwitchAsync(MobileDataSwitchEnum switchEnum)
        {
            MobileDataSwitchPostRequest m = new() { Dataswitch = (int)switchEnum };
            var url = $"http://{Modem.Host}/api/dialup/mobile-dataswitch";
            var someXmlString = XmlTools.SerializeXml(data: m);
            var stringContent = new StringContent(someXmlString, Encoding.UTF8, "text/xml");
            var result = await MakePostRequestAsync(url, stringContent, caller: $"{nameof(MobileDataSwitchAsync)} {switchEnum}");
            return result;
        }

        /// <summary>
        /// Запрашиваем статус
        /// </summary>
        /// <returns></returns>
        public async Task<MobileDataSwitchGetResponse?> MobileDataSwitchGetInfoAsync()
        {
            var url = $"http://{Modem.Host}/api/dialup/mobile-dataswitch";
            var text = await MakeGetRequestAsync(url, caller: nameof(MobileDataSwitchGetInfoAsync));
            if (string.IsNullOrEmpty(text)) return null;
            MobileDataSwitchGetResponse result = text.DeserializeXml<MobileDataSwitchGetResponse>();
            return result;
        }

        /// <summary>
        /// Переключаемся с/на 3g или 4g.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ChangeNetModeAsync(string networkMode, string networkBand, string lteBand)
        {
            NetModeRequest request = new NetModeRequest()
            {
                NetworkMode = networkMode,
                NetworkBand = networkBand,
                LTEBand = lteBand
            };
            var url = $"http://{Modem.Host}/api/net/net-mode";
            var someXmlString = XmlTools.SerializeXml(data: request);
            var stringContent = new StringContent(someXmlString, Encoding.UTF8, "text/xml");
            var result = await MakePostRequestAsync(url, stringContent, caller: nameof(ChangeNetModeAsync));
            return true;
        }

        /// <summary>
        /// Запрашиваем статус
        /// </summary>
        /// <returns></returns>
        public async Task<NetModeInfoResponse?> NetModeStatusAsync()
        {
            var url = $"http://{Modem.Host}/api/net/net-mode";
            var text = await MakeGetRequestAsync(url, caller: nameof(NetModeStatusAsync));
            if (string.IsNullOrEmpty(text)) return null;
            NetModeInfoResponse result = text.DeserializeXml<NetModeInfoResponse>();
            return result;
        }

        /// <summary>
        /// Перезагружает модем
        /// </summary>
        /// <returns></returns>
        public async Task<bool> RebootDeviceAsync()
        {
            ControlRequest q = new() { Control = ControlData.Reboot };
            var url = $"http://{Modem.Host}/api/device/control";
            var someXmlString = XmlTools.SerializeXml(data: q);
            var stringContent = new StringContent(someXmlString, Encoding.UTF8, "text/xml");
            var result = await MakePostRequestAsync(url, stringContent, caller: nameof(RebootDeviceAsync));
            return result;
        }


        /// <summary>
        /// Делаем GET-запрос.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="caller"></param>
        /// <param name="retries"></param>
        /// <returns></returns>
        private async Task<string> MakeGetRequestAsync(string url, string caller, int retries = 5)
        {
            string result = string.Empty;
            LastException = null;
            try
            {
                HttpResponseMessage? response = null;
                for (int i = 0; i < retries; i++)
                {
                    LastError = new();
                    AddHttpHeaders();
                    response = await Client.GetAsync(url);
                    result = await response.Content.ReadAsStringAsync();

                    //если вернулась ошибка, то делаем еще одну попытку
                    if (string.IsNullOrEmpty(result) | result.Contains("error"))
                    {
                        LastError = result.DeserializeXml<ApiErrors>();
                        Console.WriteLine($"Ошибка в запросе {caller}: {LastError.Code} {LastError.Message}.");
                        Thread.Sleep(1000);
                        await GetFirstTokenAsync();
                        continue;
                    }

                    if (result.ToLower().Contains(">ok")) break;
                }

                GetTokenFromHeader(response);
            }
            catch (TaskCanceledException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e) when (CheckIfSocketException(e))
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                LastException = e;
            }
            return result;
        }

        /// <summary>
        /// Делаем POST-запрос
        /// </summary>
        /// <param name="url"></param>
        /// <param name="stringContent"></param>
        /// <param name="caller"></param>
        /// <param name="retries"></param>
        /// <returns></returns>
        private async Task<bool> MakePostRequestAsync(string url, StringContent stringContent, string caller, int retries = 5)
        {
            bool result = false;
            LastException = null;
            try
            {
                HttpResponseMessage? response = null;
                for (int i = 0; i < retries; i++)
                {
                    LastError = new();
                    AddHttpHeaders();
                    response = await Client.PostAsync(url, stringContent);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    //если вернулась ошибка, то делаем еще одну попытку
                    if (string.IsNullOrEmpty(responseContent) | responseContent.Contains("error"))
                    {
                        LastError = responseContent.DeserializeXml<ApiErrors>();
                        Console.WriteLine($"Ошибка в запросе {caller}: {LastError.Code} {LastError.Message}.");
                        Thread.Sleep(1000);
                        await GetFirstTokenAsync();
                        continue;
                    }

                    if (responseContent.ToLower().Contains(">ok"))
                    {
                        result = true;
                        break;
                    }
                }
                GetTokenFromHeader(response);
            }
            catch (TaskCanceledException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e) when (CheckIfSocketException(e))
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                LastException = e;
            }

            return result;
        }



        /// <summary>
        /// Добавляем куки и csrf токен в запрос.
        /// </summary>
        /// <returns></returns>
        private bool AddHttpHeaders()
        {
            //удаляем текущие заголовки, если есть.
            if (Client.DefaultRequestHeaders.Contains("Cookie"))
            {
                Client.DefaultRequestHeaders.Remove("Cookie");
            }
            if (Client.DefaultRequestHeaders.Contains(CsrfTokenName))
            {
                Client.DefaultRequestHeaders.Remove(CsrfTokenName);
            }

            //на всякий случай проверяем, что заголовки есть
            if (string.IsNullOrEmpty(SessionInfo.SesInfo) | string.IsNullOrEmpty(SessionInfo.TokInfo))
            {
                _ = GetFirstTokenAsync().Result;
            }

            //добавляем в запрос
            Client.DefaultRequestHeaders.Add("Cookie", SessionInfo.SesInfo);
            Client.DefaultRequestHeaders.Add(CsrfTokenName, SessionInfo.TokInfo);

            return true;
        }

        /// <summary>
        /// Получаем последний CSRF токен из ответа модема
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private bool GetTokenFromHeader(HttpResponseMessage? response)
        {
            if (response != null && response.Headers.Contains(CsrfTokenName))
            {
                SessionInfo.TokInfo = response!.Headers.GetValues(CsrfTokenName).First();
                return true;
            }
            return false;
        }

        private bool CheckIfSocketException(Exception ex)
        {
            if (ex is SocketException)
            {
                var e = (SocketException) ex;
                if (e.ErrorCode == 10060) return true;
            }
            if (ex.InnerException != null) return CheckIfSocketException(ex.InnerException);
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
