namespace Common.Messages
{
    public class Message2 : JobMessage
    {
        public Message2()
        {
            Topic = "core";
        }

        public int Count { get; set; }

        public string Message { get; set; }

        public override Task<bool> ExecuteAsync()
        {
            if (Count == 500)
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
