namespace Admin.UI.Models
{
    public class QueueMessageRequest
    {
        public string Message { get; set; }

        public int Count { get; set; }

        public string Key { get; set; }

        public string? Status { get; set; }

        public int MessagesCount { get; set; }
    }
}
