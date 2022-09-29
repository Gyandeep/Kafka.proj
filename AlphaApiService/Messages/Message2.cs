namespace AlphaApiService.Messages
{
    public class Message2 : JobMessage
    {
        public Message2()
        {
            Topic = "fast";
        }

        public int Count { get; set; }
    }
}
