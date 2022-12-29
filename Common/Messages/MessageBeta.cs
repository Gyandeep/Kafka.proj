namespace Common.Messages
{
    public class MessageBeta : JobMessage
    {
        public MessageBeta()
        {
            Topic = "mainqueue";
        }

        public override string Topic { get; set; }

        public override Task<bool> ExecuteAsync()
        {
            Console.WriteLine("MessageBeta Executing");
            return Task.FromResult(true);
        }
    }
}
