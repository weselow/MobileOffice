namespace Dashboard.Backoffice.Huawei.Models.Net
{
    public static class NetworkMode
    {
        /// <summary>
        /// Автоматический (все сети)
        /// </summary>
        public const string Auto = "00";

        /// <summary>
        /// Только 2G
        /// </summary>
        public const string Only2G = "01";

        /// <summary>
        /// Только 3G
        /// </summary>
        public const string Only3G = "02";

        /// <summary>
        /// Только 4G (LTE)
        /// </summary>
        public const string Only4G = "03";

        /// <summary>
        /// Без изменений
        /// </summary>
        public const string NoChanges = "00";

    }
}
