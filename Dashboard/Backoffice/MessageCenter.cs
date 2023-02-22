namespace Dashboard.Backoffice
{
    public static class MessageCenter
    {
        public static List<string> Warnings { get; set; } = new();
        public static List<string> Errors { get; set; } = new();
        public static List<string> Info { get; set; } = new();
    }
}
