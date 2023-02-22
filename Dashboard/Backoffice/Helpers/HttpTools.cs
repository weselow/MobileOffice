using System.Net;
using Dashboard.Backoffice.Models;

namespace Dashboard.Backoffice.Helpers
{
    public static class HttpTools
    {
        private static string Url { get; set; } = "http://api.ipify.org";
        public static Dictionary<string, string> GetHeaders(HttpResponseMessage response)
        {
            Dictionary<string, string> result = new();
            foreach (var header in response.Headers)
            {
                if (!result.ContainsKey(header.Key))
                {
                    result.Add(header.Key.ToString(), header.Value.First().ToString() ?? "");
                }
                else
                {
                    result[header.Key.ToString()] += Environment.NewLine + header.Value.ToString();
                }

            }
            return result;
        }
        private static SocketsHttpHandler GetHandler()
        {
            SocketsHttpHandler handler = new()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = true,
                PooledConnectionIdleTimeout = TimeSpan.Zero
            };
            return handler;
        }
        private static SocketsHttpHandler GetHandler(Proxy proxy)
        {
            SocketsHttpHandler handler = new()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = true,
                PooledConnectionIdleTimeout = TimeSpan.Zero,

                Proxy = new WebProxy($"{proxy.Type}://{proxy.Ip}:{proxy.Port}")
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(proxy.Login, proxy.Password)
                },
                UseProxy = true,
                PreAuthenticate = true
            };
            return handler;
        }

        public static HttpClient GetHttpClient(Proxy? proxy = null, int timeout = 3 * 1000)
        {
            var handler = proxy != null
                ? GetHandler(proxy)
                : GetHandler();
            HttpClient client = new(handler)
            {
                Timeout = TimeSpan.FromMilliseconds(5 * 1000)
            };
            return client;
        }

        public static async Task<string> GetExternalIp(Proxy proxy)
        {
            var result = string.Empty;
            try
            {
                using HttpClient client = GetHttpClient(proxy: proxy, timeout: 5 * 1000);
                var response = await client.GetStringAsync(Url);
                if (!string.IsNullOrEmpty(response) & response.Length < 20) { result = response; }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return result;
        }
    }
}
