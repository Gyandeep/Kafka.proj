namespace Common.Messages
{
    public class BadMessage : JobMessage
    {
        public BadMessage()
        {
            Topic = "mainqueue";
        }

        public override string Topic { get; set; }

        public override Task<bool> ExecuteAsync()
        {
            //Console.WriteLine("Bad Message executing");
            return Task.FromResult(false);
        }
    }
}
