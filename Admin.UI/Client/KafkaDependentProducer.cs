﻿namespace Admin.UI.Client
{
    using Confluent.Kafka;

    public class KafkaDependentProducer<K, V>
    {
        IProducer<K, V> kafkaHandle;

        public KafkaDependentProducer(KafkaClientHandle handle)
        {
            kafkaHandle = new DependentProducerBuilder<K, V>(handle.Handle).Build();
        }

        /// <summary>
        ///     Asychronously produce a message and expose delivery information
        ///     via the returned Task. Use this method of producing if you would
        ///     like to await the result before flow of execution continues.
        /// <summary>
        public Task<DeliveryResult<K, V>> ProduceAsync(string topic, Message<K, V> message)
            => this.kafkaHandle.ProduceAsync(topic, message);

        /// <summary>
        ///     Asynchronously produce a message and expose delivery information
        ///     via the provided callback function. Use this method of producing
        ///     if you would like flow of execution to continue immediately, and
        ///     handle delivery information out-of-band.
        /// </summary>
        public void Produce(string topic, Message<K, V> message, Action<DeliveryReport<K, V>> deliveryHandler = null)
            => this.kafkaHandle.Produce(topic, message, deliveryHandler);

        public void Flush(TimeSpan timeout)
            => this.kafkaHandle.Flush(timeout);

        public string ReadContent()
        {
            var files = Directory.GetFiles(@$"C:\Users\GyandeepNeema\AppData\Local\Stats");

            if (files != null && files.Any())
            {
                var content = File.ReadAllText(files.Last());
                return content; 
            }

            return String.Empty;
        }
    }
}
