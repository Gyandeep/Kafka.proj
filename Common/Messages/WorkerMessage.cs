namespace Common.Messages
{
    public class WorkerMessage
    {
        public int Count { get; set; }

        public string? Message { get; set; }

        public string Topic => "core";
    }
}
