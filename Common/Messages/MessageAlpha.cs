namespace Common.Messages
{
    public class MessageAlpha : JobMessage
    {
        public MessageAlpha()
        {
            Topic = "mainqueue";
        }

        public override string Topic { get; set; }

        public override Task<bool> ExecuteAsync()
        {
            Console.WriteLine("MessageAlpha Executing");
            return Task.FromResult(true);
        }
    }
}
