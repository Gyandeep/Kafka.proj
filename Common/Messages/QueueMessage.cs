namespace Common.Messages
{
    public class QueueMessage
    {
        public QueueMessage()
        {
            RetryCounter = 2;
            CreatedUtc = DateTime.UtcNow;
        }

        public string Id { get; set; }

        public int RetryCounter { get; set; }

        public JobMessage Message { get; set; }

        public DateTime? RetryAfterUtc { get; set; }

        public DateTime? RetryQueueTimeUtc { get; set; }

        public double? SleepForSeconds { get; set; }

        public DateTime ScheduledDateTimeUtc { get; set; }

        public bool IsSpecialMessage => SleepForSeconds.HasValue;

        public DateTime CreatedUtc { get; set; }
    }
}
