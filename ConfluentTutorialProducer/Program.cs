using Confluent.Kafka;

namespace ConfluentTutorialProducer
{
    internal class Program
    {
        private const int kafkaPort = 9092;

        public static void Main(string[] args)
        {
            const string topic = "purchases";

            string[] users = { "eabara", "jsmith", "sgarcia", "jbernard", "htanaka", "awalther" };
            string[] items = { "book", "alarm clock", "t-shirts", "gift card", "batteries" };

            var config = new ProducerConfig()
            {
                // User-specific properties that you must set
                BootstrapServers = $"localhost:{kafkaPort}",
                // Fixed properties
                Acks = Acks.All,
            };

            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                // Produce messages
                var numProduced = 0;
                Random rnd = new Random();
                const int numMessages = 10;
                for (int i = 0; i < numMessages; i++)
                {
                    // Pick a user and item randomly
                    var user = users[rnd.Next(users.Length)];
                    var item = items[rnd.Next(items.Length)];

                    producer.Produce
                    (
                        topic,
                        new Message<string, string>()
                        {
                            Key = user,
                            Value = item,
                        },
                        (deliveryReport) =>
                        {
                            if (deliveryReport.Error != ErrorCode.NoError)
                            {
                                Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                            }
                            else
                            {
                                Console.WriteLine($"Produced event to topic {topic}: key = {user,-10}, value = {item}");
                                numProduced += 1;
                            }
                        }
                    );
                }

                producer.Flush(TimeSpan.FromSeconds(10));
                Console.WriteLine($"{numProduced} messages were produced to topic {topic}.");
            }
        }
    }
}