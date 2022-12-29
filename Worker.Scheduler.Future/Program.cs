using Common;
using Common.Messages;

using Confluent.Kafka;

using Newtonsoft.Json;

namespace Worker.Scheduler.Future
{
    public class Program
    {
        static void Main(string[] args)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable("BOOTSTRAP_SERVERS"), //"192.168.1.2:9092",//"192.168.1.12:9092",
                GroupId = "kafka-futurescheduler",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                MaxPollIntervalMs = 960000

            };

            string sameDayTopic = "samedayqueue";// args[0];
            string futureTopic = "futurequeue";
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };

            //Queue Special message
            var specialMessage = new QueueMessage()
            {
                SleepForSeconds = 900
            };

            QueueToTopic(specialMessage, futureTopic).Wait();

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe(futureTopic);
                try
                {
                    while (true)
                    {
                        var consumeResult = consumer.Consume(cts.Token);
                        var queueMessage = JsonUtility.DeserializeMessage<QueueMessage>(consumeResult.Message.Value);

                        if (queueMessage.SleepForSeconds.HasValue)
                        {
                            //var sleepDuration = DateTime.UtcNow.AddSeconds(queueMessage.SleepForSeconds.Value) - DateTime.UtcNow;
                            Console.WriteLine($"Committing {consumeResult.Offset.Value} & Sleeping for 15 mins, {DateTime.UtcNow}");
                            consumer.Commit(consumeResult);
                            Thread.Sleep(900000);
                        }

                        //Console.WriteLine($"Executing msg: {queueMessage.Id} after {(DateTime.UtcNow - queueMessage.RetryQueueTimeUtc).GetValueOrDefault().TotalSeconds} secs");

                        if (!queueMessage.IsSpecialMessage)
                        {
                            if (IsMessageScheduledForNextSlot(queueMessage.ScheduledDateTimeUtc))
                            {
                                Console.WriteLine($"Queuing to Sameday Queue. Id: {queueMessage.Id} ScheduledAt: {queueMessage.ScheduledDateTimeUtc} UtcNow: {DateTime.UtcNow}");
                                QueueToTopic(queueMessage, sameDayTopic).Wait();
                            }
                            else
                            {
                                Console.WriteLine($"Queuing back to Future Queue. Id: {queueMessage.Id} ScheduledAt: {queueMessage.ScheduledDateTimeUtc} UtcNow: {DateTime.UtcNow}");
                                QueueToTopic(queueMessage, futureTopic).Wait();
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
                            Console.WriteLine($"Scheduling special message back to queue UtcNow: {DateTime.Now}");
                            QueueToTopic(specialMessage, futureTopic).Wait();
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

        private static bool IsMessageScheduledForNextSlot(DateTime scheduledDateTimeUtc)
        {
            var nextslot = DateTime.UtcNow.AddMinutes(15);

            return scheduledDateTimeUtc <= nextslot;
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
