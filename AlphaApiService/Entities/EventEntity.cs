using AlphaApiService.Messages;

namespace AlphaApiService.Entities
{
    public class EventEntity
    {
        public string Message { get; set; }

        public int Count { get; set; }

        public string Key { get; set; }

        public string? Status { get; set; }
    }
}
