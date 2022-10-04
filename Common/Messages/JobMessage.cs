using System.Runtime.Serialization;

namespace Common.Messages
{
    [KnownType(typeof(Message2))]
    [KnownType(typeof(Message1))]
    public class JobMessage
    {
        public string Id => Guid.NewGuid().ToString();

        public string Topic { get; set; }

        public string Key { get; set; }

        public virtual Task<bool> ExecuteAsync() 
        {
            return Task.FromResult(true);
        }
    }
}
