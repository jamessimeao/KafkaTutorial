using Confluent.Kafka;

namespace ConfluentTutorialConsumer
{
    internal class Program
    {
        private const int kafkaPort = 9092;

        public static void Main(string[] args)
        {
            var config = new ConsumerConfig()
            {
                // User-specific properties that you must set
                BootstrapServers = $"localhost:{kafkaPort}",

                // Fixed properties
                GroupId = "kafka-dotnet-getting-started",
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };

            const string topic = "purchases";

            // CancellationTokenSource is from Threading, not Kafka
            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true; // prevent the process from terminating
                cts.Cancel();
            };

            using (var consumer = new ConsumerBuilder<string, string>(config).Build())
            {
                consumer.Subscribe(topic);

                try
                {
                    while(true)
                    {
                        var cr = consumer.Consume(cts.Token); // Token is the cancellation token
                        Console.WriteLine($"Consumed event from topic {topic}: key = {cr.Message.Key, -10}, value = {cr.Message.Value}");
                    }
                }
                catch(OperationCanceledException ex)
                {
                    // Ctrl-C was pressed
                }
                finally
                {
                    consumer.Close();
                }
            }
        }
    }
}