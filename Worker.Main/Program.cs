//// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using Common;
using Common.Messages;
using Confluent.Kafka;

namespace Worker.Retry
{
    public class Program
    {
        static void Main(string[] args)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable("BOOTSTRAP_SERVERS"),//"192.168.1.2",// "192.168.1.4:9092",
                GroupId = "kafka-mainqueue",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
            };

            string topic = "mainqueue";// args[0];
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe(topic);
                try
                {
                    while (true)
                    {
                        var consumeResult = consumer.Consume(cts.Token);
                        var queueMessage = JsonUtility.DeserializeMessage<QueueMessage>(consumeResult.Message.Value);

                        if (!queueMessage.Message.ExecuteAsync().Result)
                        {
                            var waitTime = DateTime.UtcNow.AddMinutes(1);
                            Console.WriteLine($"Queuing msg: {queueMessage.Id} to Retry Queue with 1 min delay. wait until: {waitTime:G}");
                            queueMessage.RetryAfterUtc = waitTime;
                            QueueToRetry(queueMessage).Wait();
                        }
                        else
                        {
                            Console.WriteLine($"Executed message successfully!!! Id: {queueMessage.Id}, ScheduledAt: {queueMessage.ScheduledDateTimeUtc}, ExecutedAt: {DateTime.UtcNow}");
                        }

                        try
                        {
                            consumer.Commit(consumeResult);
                            //Console.WriteLine($"Commited offset: {consumeResult.Offset.Value}");
                        }
                        catch (KafkaException e)
                        {
                            Console.WriteLine($"Commit error: {e.Error.Reason}");
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

        private static async Task QueueToRetry(QueueMessage queueMessage)
        {
            var config = new List<KeyValuePair<string, string>>();
            config.Add(new KeyValuePair<string, string>("bootstrap.servers", Environment.GetEnvironmentVariable("BOOTSTRAP_SERVERS")));

            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                queueMessage.RetryQueueTimeUtc = DateTime.UtcNow;
                _ = await producer.ProduceAsync("retryqueue", new Message<string, string> { Value = JsonUtility.SerializeMessage(queueMessage) });
            }
        }
    }
}
