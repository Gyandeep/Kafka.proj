using System.Runtime.Serialization;

namespace AlphaApiService.Messages
{
    [KnownType(typeof(Message2))]
    [KnownType(typeof(Message1))]
    public abstract class JobMessage
    {
        public string Id => Guid.NewGuid().ToString();

        public string Topic { get; set; }

        public string Key { get; set; }
    }
}
