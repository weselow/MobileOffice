namespace Dashboard.Backoffice.Huawei.Models.Device
{
    public static class ControlData
    {
        /// <summary>
        /// Перезагрузка устройства
        /// </summary>
        public const string Reboot = "1";

        /// <summary>
        /// Восстановление конфигурации (нет информации о том, как указать файл с сохраненной конфигурацией. Возможно, используется файл http://192.168.8.1/nvram.bak)
        /// </summary>
        public const string Reset = "2";

        /// <summary>
        /// Конфигурация резервного копирования (конфигурация доступна по адресу http://192.168.8.1/nvram.bak. Файл в кодировке base64)
        /// </summary>
        public const string BackupConfig = "3";

        /// <summary>
        /// Выключение устройства
        /// </summary>
        public const string PowerOff = "4";
    }
}
