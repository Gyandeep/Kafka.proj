namespace Admin.UI.Models
{
    public class CreateTopicRequest
    {
        public string? TopicName { get; set; }

        //public short? ReplicationFactor { get; set; }

        public int? NumberOfPartitions { get; set; }
    }
}
