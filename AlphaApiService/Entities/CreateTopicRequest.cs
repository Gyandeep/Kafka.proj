namespace AlphaApiService.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class CreateTopicRequest
    {
        [Required]
        public string TopicName { get; set; }

        //public short? ReplicationFactor { get; set; }

        public int? NumberOfPartitions { get; set; }
    }
}
