namespace Common.Messages
{
    public abstract class JobMessage
    {
        protected JobMessage()
        {
        }

        public string Id => Guid.NewGuid().ToString();

        public abstract string Topic { get; set; }

        public string Key { get; set; }

        public abstract Task<bool> ExecuteAsync();
    }
}
