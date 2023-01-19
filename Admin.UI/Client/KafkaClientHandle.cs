namespace Admin.UI.Client
{
    using System.Collections.Concurrent;

    using Common;

    using Confluent.Kafka;

    public class KafkaClientHandle : IDisposable
    {
        IProducer<byte[], byte[]> kafkaProducer;
        ConcurrentDictionary<string, List<TopicMetadata>> topicMetadataCache;

        public KafkaClientHandle(IConfiguration config)
        {
            var conf = new ProducerConfig();
            topicMetadataCache = new ConcurrentDictionary<string, List<TopicMetadata>>();
            config.GetSection("Kafka:ProducerSettings").Bind(conf);
            //var producerBuilder = new ProducerBuilder<byte[], byte[]>(conf).SetStatisticsHandler((producer, s) => {
            //    try
            //    {
            //        var metadata = JsonUtility.DeserializeMessage<Metadata>(s);

            //        foreach (var item in metadata.Topics)
            //        {
            //            if (!topicMetadataCache.ContainsKey(item.Topic))
            //            {
            //                topicMetadataCache.TryAdd(item.Topic, new List<TopicMetadata>() { item });
            //            }
            //            else
            //            {
            //                topicMetadataCache[item.Topic].Add(item);
            //            }

            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex);
            //    }
            //});
            this.kafkaProducer = new ProducerBuilder<byte[], byte[]>(conf).Build();// producerBuilder.Build();
        }

        public Handle Handle { get => this.kafkaProducer.Handle; }

        public ConcurrentDictionary<string, List<TopicMetadata>> TopicMetadataCache => topicMetadataCache;

        public void Dispose()
        {
            // Block until all outstanding produce requests have completed (with or
            // without error).
            kafkaProducer.Flush();
            kafkaProducer.Dispose();
        }
    }
}
