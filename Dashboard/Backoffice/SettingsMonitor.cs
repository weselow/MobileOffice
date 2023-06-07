using System.Text.Json;
using Dashboard.Backoffice.Models;
using System.Timers;
using System.Text.Json.Serialization;

namespace Dashboard.Backoffice
{
    public class SettingsMonitor
    {
        private Dictionary<int, Modem> Modems { get; set; }
        private System.Timers.Timer Atimer { get; set; }
        private static string SettingsPath { get; set; } = null!;

        private static JsonSerializerOptions DefaultJsonOptions { get; set; } = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };
        public SettingsMonitor(Dictionary<int, Modem> modems)
        {
            SettingsPath = Path.Combine(AppContext.BaseDirectory, "_settings-modems.json");
            Modems = modems;
            Atimer = new System.Timers.Timer(60 * 1000) { AutoReset = true, Enabled = true };
            Atimer.Elapsed += SaveAsync;
        }

        public bool LoadSettings()
        {
            //проверяем файл
            if (!File.Exists(SettingsPath))
            {
                //файл настроек не существует
                MessageCenter.Errors.Add("Файл настроек не существует, восстанавливаем значения по-умолчанию.");
                LoadDefaults();
                return false;
            }

            //читаем файл
            string json;
            try
            {
                json = File.ReadAllText(SettingsPath);
                if (string.IsNullOrEmpty(json)) throw new Exception("Json string is empty!");
            }
            catch (Exception e)
            {
                MessageCenter.Errors.Add($"Ошибка при чтении файла настроек - {e.Message}.");
                return false;
            }

            //десериализуем
            List<Modem>? data = new List<Modem>();
            try
            {
                data = JsonSerializer.Deserialize<List<Modem>>(json, DefaultJsonOptions);
                if (data == null) throw new Exception("Decerialyzed list Data is null!");
            }
            catch (Exception e)
            {
                MessageCenter.Errors.Add($"Ошибка при десериализации настроек - {e.Message}.");
                return false;
            }

            //добавляем в коллекцию
            foreach (var t in data)
            {
                Modems.Add(t.Port, t);
            }

            return true;
        }
        private void LoadDefaults()
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
        }

        public async void SaveAsync(Object? source = null, ElapsedEventArgs? e = null)
        {
            //convert to json
            string json;
            List<Modem> data = Modems.Values.ToList();
            try
            {
                json = JsonSerializer.Serialize(data, DefaultJsonOptions);
            }
            catch (Exception ex)
            {
                MessageCenter.Errors.Add($"Ошибка при сериализации Modems в Json: {ex.Message}.");
                return;
            }

            //save
            try
            {
                await File.WriteAllTextAsync(SettingsPath, json);
            }
            catch (Exception ex)
            {
                MessageCenter.Errors.Add($"Ошибка при сохранении Modems в файл: {ex.Message}.");
            }
        }
    }
}
