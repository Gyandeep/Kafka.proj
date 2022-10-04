namespace Common.Messages
{
    public class Message1 : JobMessage
    {
        public Message1()
        {
            Topic = "core";
        }

        public string Message { get; set; }

        public override Task<bool> ExecuteAsync()
        {
            return Task.FromResult(true);
        }
    }
}
