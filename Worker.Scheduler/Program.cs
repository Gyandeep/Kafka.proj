using Common;
using Common.Messages;

using Confluent.Kafka;

using Newtonsoft.Json;

namespace Worker.Scheduler
{
    public class Program
    {
        static void Main(string[] args)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable("BOOTSTRAP_SERVERS"), //"192.168.1.2:9092",//"192.168.1.12:9092",
                GroupId = "kafka-scheduler",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,

            };

            string schedulingTopic = "schedulingqueue";// args[0];
            string sameDayTopic = "samedayqueue";
            string futureTopic = "futurequeue";
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe(schedulingTopic);
                try
                {
                    while (true)
                    {
                        var consumeResult = consumer.Consume(cts.Token);
                        var queueMessage = JsonUtility.DeserializeMessage<QueueMessage>(consumeResult.Message.Value);
                        var producerConfig = new List<KeyValuePair<string, string>>();
                        producerConfig.Add(new KeyValuePair<string, string>("bootstrap.servers", Environment.GetEnvironmentVariable("BOOTSTRAP_SERVERS")));
                        producerConfig.Add(new KeyValuePair<string, string>("transactional.id", "schedulerworker"));

                        using (var producer = new ProducerBuilder<string, string>(producerConfig).Build())
                        {
                            producer.InitTransactions(TimeSpan.FromSeconds(10));
                            
                            try
                            {
                                producer.BeginTransaction();

                                if (IsMessageScheduledForFuture(queueMessage.ScheduledDateTimeUtc))
                                {
                                    Console.WriteLine($"Queuing to future Queue. Id: {queueMessage.Id} ScheduledAt: {queueMessage.ScheduledDateTimeUtc} UtcNow: {DateTime.UtcNow}");
                                    //QueueToTopic(queueMessage, futureTopic).Wait();
                                    queueMessage.RetryQueueTimeUtc = DateTime.UtcNow;
                                    producer.Produce(futureTopic, new Message<string, string> { Value = JsonUtility.SerializeMessage(queueMessage) });
                                }
                                else
                                {
                                    Console.WriteLine($"Queuing to sameday Queue. Id: {queueMessage.Id} ScheduledAt: {queueMessage.ScheduledDateTimeUtc} UtcNow: {DateTime.UtcNow}");
                                    //QueueToTopic(queueMessage, sameDayTopic).Wait();
                                    producer.Produce(sameDayTopic, new Message<string, string> { Value = JsonUtility.SerializeMessage(queueMessage) });
                                }

                                //consumer.Commit(consumeResult);
                                producer.SendOffsetsToTransaction(new List<TopicPartitionOffset> { consumeResult.TopicPartitionOffset }, consumer.ConsumerGroupMetadata, TimeSpan.FromSeconds(10));
                                producer.CommitTransaction();
                                Console.WriteLine($"Commited offset: {consumeResult.Offset.Value}");
                            }
                            catch (KafkaException e)
                            {
                                Console.WriteLine($"Commit error: {e.Error.Reason}");
                                producer.AbortTransaction();
                            }
                        }
                    }
                }
                catch (OperationCanceledException oe)
                {
                    Console.WriteLine(oe);
                    // Ctrl-C was pressed.
                }
                finally
                {
                    consumer.Close();
                }
            }
        }

        private static bool IsMessageScheduledForFuture(DateTime scheduledDateTimeUtc)
        {
            var future = DateTime.UtcNow.AddMinutes(15);

            return scheduledDateTimeUtc > future;
        }

        private static async Task QueueToTopic(QueueMessage queueMessage, string topic)
        {
            var config = new List<KeyValuePair<string, string>>();
            config.Add(new KeyValuePair<string, string>("bootstrap.servers", Environment.GetEnvironmentVariable("BOOTSTRAP_SERVERS")));

            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                queueMessage.RetryQueueTimeUtc = DateTime.UtcNow;
                _ = await producer.ProduceAsync(topic, new Message<string, string> { Value = JsonUtility.SerializeMessage(queueMessage) });
            }
        }
    }
}
