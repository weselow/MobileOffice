namespace Dashboard.Backoffice.Huawei.Models.Net
{
    public static class NetModeData
    {
        public static Dictionary<string, string> NetworkMode { get; set; } = new();
        public static Dictionary<string, string> NetworkBand { get; set; } = new();
        public static Dictionary<string, string> LTEBand { get; set; } = new();

        static NetModeData()
        {
            SetData();
        }
        private static void SetData()
        {
            NetworkMode = new()
            {
                ["00"] = "автоматический (все сети)",
                ["01"] = "Только 2G",
                ["02"] = "Только 3G",
                ["03"] = "Только 4G (LTE)",
                ["99"] = "без изменений"
            };

            NetworkBand = new()
            {

                ["80"] = "GSM1800",
                ["300"] = "GSM900",
                ["80000"] = "GSM850",
                ["200000"] = "GSM1900",
                ["400000"] = "УТМС Б1 (2100)",
                ["800000"] = "УТМС Б2 (1900)",
                ["4000000"] = "УТМС Б5 (850)",
                ["2000000000000"] = "УТМС Б8 (900)",
                ["1"] = "BC0A",
                ["2"] = "БК0Б",
                ["4"] = "BC1",
                ["8"] = "BC2",
                ["10"] = "BC3",
                ["20"] = "BC4",
                ["40"] = "BC5",
                ["400"] = "BC6",
                ["800"] = "BC7",
                ["1000"] = "BC8",
                ["2000"] = "BC9",
                ["4000"] = "BC10",
                ["8000"] = "BC11",
                ["10000000"] = "BC12",
                ["20000000"] = "BC13",
                ["80000000"] = "BC14",
                ["3FFFFFFF"] = "все поддерживается"
            };

            LTEBand = new()
            {
                ["1"] = "B1 (FDD 2100)",
                ["4"] = "B3 (FDD 1800)",
                ["40"] = "B7 (FDD 2600)",
                ["80"] = "B8 (FDD 900)",
                ["80000"] = "B20 (FDD 800)",
                ["2000000000"] = "B38 (TDD 2600)",
                ["8000000000"] = "B40 (TDD 2300)",
                ["800C5"] = "Диапазоны ЕС (LTE Band 1, 3, 7, 8, 20)",
                ["800D5"] = "Диапазоны ЕС/Азии/Африки (LTE Band 1, 3, 5, 7, 8, 20)",
                ["7FFFFFFFFFFFFF"] = "все диапазоны"
            };
        }
    }
}
