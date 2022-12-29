//// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using Confluent.Kafka;
using static Confluent.Kafka.ConfigPropertyNames;
using System.Threading;
using Common.Messages;
using Newtonsoft.Json;

namespace Worker.Atleast.Once
{
    public class Program
    {
        static void Main(string[] args)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "192.168.1.3:9092",
                GroupId = "kafka-core",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            string topic = "core";// args[0];

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
                        var message = JsonConvert.DeserializeObject<WorkerMessage>(consumeResult.Message.Value);
                        Console.WriteLine($"Message: {message.Message}, Partition: {consumeResult.Partition.Value}, Offset: {consumeResult.Offset.Value}");
                        
                        //if (message.Count == 2) {
                        //    // throw new Exception("failing Message 2");
                        //    Console.WriteLine("Crash---Causing application shut down-------");
                        //    break;
                           
                        //}

                        try
                        {
                            Console.WriteLine($"Commiting offset: {consumeResult.Offset.Value}");
                            consumer.Commit(consumeResult);
                        }
                        catch (KafkaException e)
                        {
                            Console.WriteLine($"Commit error: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ctrl-C was pressed.
                }
                finally
                {
                    consumer.Close();
                }
            }
        }
    }
}