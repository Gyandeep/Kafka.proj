//// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using Common;
using Common.Messages;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace Worker.Retry
{
    public class Program
    {
        static void Main(string[] args)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "192.168.1.4:9092",
                GroupId = "kafka-retryqueue",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                
            };

            string topic = "retryqueue";// args[0];
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

                        //Console.WriteLine($"Msg: {queueMessage.Id}, Utc Now: {DateTime.UtcNow}, RetryAfterUtc: {queueMessage.RetryAfterUtc.GetValueOrDefault()}");
                        //Check if it time for message to execute otherwise wait until then.
                        if (DateTime.UtcNow < queueMessage.RetryAfterUtc.GetValueOrDefault())
                        {
                            var sleepDuration = queueMessage.RetryAfterUtc.GetValueOrDefault() - DateTime.UtcNow;
                            Console.WriteLine($"Sleeping for {sleepDuration.TotalSeconds}");
                            Thread.Sleep(sleepDuration);
                        }

                        Console.WriteLine($"Executing msg: {queueMessage.Id} after {(DateTime.UtcNow - queueMessage.RetryQueueTimeUtc).GetValueOrDefault().TotalSeconds} secs");

                        if (!queueMessage.Message.ExecuteAsync().Result)
                        {
                            if (queueMessage.RetryCounter > 0)
                            {
                                //    // push it to retry topic
                                var waitTime = DateTime.UtcNow.AddMinutes(1);
                                Console.WriteLine($"Queuing msg: {queueMessage.Id} to Retry Queue with 1 min delay. wait until: {waitTime:G}, Retry Counter: {queueMessage.RetryCounter}");
                                queueMessage.RetryCounter--;
                                queueMessage.RetryAfterUtc = waitTime;
                                QueueToRetry(queueMessage).Wait();
                            }
                            else
                            {
                                Console.WriteLine($"Queuing msg: {queueMessage.Id} to Dead Letter Queue for manual intervention");
                            }
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
            config.Add(new KeyValuePair<string, string>("bootstrap.servers", "192.168.1.4:9092"));
            
            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                queueMessage.RetryQueueTimeUtc = DateTime.UtcNow;
                var delivery = await producer.ProduceAsync("retryqueue", new Message<string, string> { Value = JsonUtility.SerializeMessage(queueMessage) });
            }
        }
    }
}
