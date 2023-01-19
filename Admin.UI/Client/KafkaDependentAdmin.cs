﻿namespace Admin.UI.Client
{
    using Confluent.Kafka;
    using Confluent.Kafka.Admin;

    public class KafkaDependentAdmin
    {
        IAdminClient kafkaAdminHandle;

        public KafkaDependentAdmin(AdminClientHandle handle)
        {
            kafkaAdminHandle = new DependentAdminClientBuilder(handle.Handle).Build();
        }

        public Task CreateTopicAsync(IEnumerable<TopicSpecification> specifications, CreateTopicsOptions? createTopicsOptions = null)
            => kafkaAdminHandle.CreateTopicsAsync(specifications, createTopicsOptions);

        public Metadata GetMetadata() => kafkaAdminHandle.GetMetadata(TimeSpan.FromSeconds(20));
    }
}
