using Common;
using Common.Messages;

using Confluent.Kafka;

using Newtonsoft.Json;

namespace Worker.Scheduler.Sameday
{
    public class Program
    {
        static void Main(string[] args)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable("BOOTSTRAP_SERVERS"),//"192.168.1.2:9092",//"192.168.1.12:9092",
                GroupId = "kafka-samedayscheduler",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                MaxPollIntervalMs = 360000

            };

            string sameDayTopic = "samedayqueue";// args[0];
            string mainTopic = "mainqueue";
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };

            //Queue Special message
            var specialMessage = new QueueMessage()
            {
                SleepForSeconds = 300
            };

            QueueToTopic(specialMessage, sameDayTopic).Wait();

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe(sameDayTopic);
                try
                {
                    while (true)
                    {
                        var consumeResult = consumer.Consume(cts.Token);
                        var queueMessage = JsonUtility.DeserializeMessage<QueueMessage>(consumeResult.Message.Value);
                        
                        if (queueMessage.IsSpecialMessage)
                        {
                            //var sleepDuration = DateTime.UtcNow.AddSeconds(queueMessage.SleepForSeconds.Value) - DateTime.UtcNow;
                            Console.WriteLine($"Committing  {consumeResult.Offset.Value} & Sleeping for 5 mins {DateTime.UtcNow}");
                            consumer.Commit(consumeResult);
                            Thread.Sleep(300000);
                        }

                        //Console.WriteLine($"Executing msg: {queueMessage.Id} after {(DateTime.UtcNow - queueMessage.RetryQueueTimeUtc).GetValueOrDefault().TotalSeconds} secs");

                        if (!queueMessage.IsSpecialMessage)
                        {
                            if (IsMessageScheduledNow(queueMessage.ScheduledDateTimeUtc))
                            {
                                Console.WriteLine($"Queuing to main Queue. Id: {queueMessage.Id} ScheduledAt: {queueMessage.ScheduledDateTimeUtc} UtcNow: {DateTime.UtcNow}");
                                QueueToTopic(queueMessage, mainTopic).Wait();
                            }
                            else
                            {
                                Console.WriteLine($"Queuing back to sameday Queue. Id: {queueMessage.Id} ScheduledAt: {queueMessage.ScheduledDateTimeUtc} UtcNow: {DateTime.UtcNow}");
                                QueueToTopic(queueMessage, sameDayTopic).Wait();
                            }
                            try
                            {
                                consumer.Commit(consumeResult);
                                Console.WriteLine($"Commited offset: {consumeResult.Offset.Value}");
                            }
                            catch (KafkaException e)
                            {
                                Console.WriteLine($"Commit error: {e.Error.Reason}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Scheduling special message back to queue UtcNow: {DateTime.UtcNow}");
                            QueueToTopic(specialMessage, sameDayTopic).Wait();
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

        private static bool IsMessageScheduledNow(DateTime scheduledDateTimeUtc)
        {
            return scheduledDateTimeUtc <= DateTime.UtcNow;
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
