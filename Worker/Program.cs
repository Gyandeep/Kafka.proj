// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace Worker
{
    public class Program
    {
        static void Main(string[] args)
        {
            //if (args.Length < 1)
            //{
            //    Console.WriteLine("Please provide the configuration file path as a command line argument");
            //}

            //IConfiguration configuration = new ConfigurationBuilder()
            //    .AddIniFile("./Configuration/kafka.properties")
            //    .Build();

            //configuration["group.id"] = "kafka-core";
            //configuration["auto.offset.reset"] = "earliest";

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
                    while (true)
                    {
                        var cr = consumer.Consume(cts.Token);
                        Console.WriteLine($"Consumed event from topic {topic} with key {cr.Message.Key,-10} and value {cr.Message.Value}");
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
