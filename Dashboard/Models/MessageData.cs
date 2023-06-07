namespace Dashboard.Models
{
    public class MessageData
    {
        public List<string> danger { get; set; } = new();
        public List<string> Warning { get; set; } = new();
        public List<string> Info { get; set; } = new();
    }
}
