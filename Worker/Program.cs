// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using Common.Messages;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.WebSockets;

namespace Worker
{
    public class Program
    {
        static void Main(string[] args)
        {
            var configs = new List<KeyValuePair<string, string>>();

            configs.Add(new KeyValuePair<string, string>("bootstrap.servers", "192.168.1.5:9092"));
            configs.Add(new KeyValuePair<string, string>("group.id", "kafka-core"));
            configs.Add(new KeyValuePair<string, string>("auto.offset.reset", "earliest"));

            string topic = "core";// args[0];

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };

            using (var consumer = new ConsumerBuilder<string, string>(configs).Build())
            {
                consumer.Subscribe(topic);
                try
                {
                   // int count
                    while (true)
                    {
                        var cr = consumer.Consume(cts.Token);
                        Console.WriteLine(
                            $"Consumed: topic: {topic} key: {cr.Message.Key} Offset: {cr.Offset} Partition: {cr.Partition.Value}");

                        var message = JsonConvert.DeserializeObject<Message2>(cr.Message.Value);

                        //if (message is Message2) 
                        //{
                            //var m2 = message as Message2;

                            if (message.Count == 5)
                            {
                                var msg = $"Message: {message.Message} Offset {cr.Offset} in partition {cr.Partition.Value} failed.";
                                Console.WriteLine(msg);
                                throw new Exception(msg);
                            }
                        //}
                        
                        _ = message.ExecuteAsync().Result;

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
