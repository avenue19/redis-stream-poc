using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace stream_sub
{
    class Program
    {
        private const string RedisConnectionString = "localhost:6379";
        private static readonly ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(RedisConnectionString);
        private const string StreamId = "events_stream";
        private const string ConsumerGroup = "events_consumer_group";

        static async Task Main(string[] args)
        {
            // TODO: use a fixed set of consumer IDs or delete consumers after each run
            var consumerId = Guid.NewGuid().ToString();
            Console.WriteLine($"Consumer {consumerId} starting...");
            var db = redis.GetDatabase();

            Random rand = new();

            while (true)
            {
                var msgs = await db.StreamReadGroupAsync(StreamId, ConsumerGroup, consumerId, ">", count: 1);
                if (msgs.Length > 0)
                {
                    Console.WriteLine($"Received message {msgs[0].Values[0]}");
                    await Task.Delay(3000);
                    if (rand.Next(0, 10) > 2)
                    {
                        Console.WriteLine($"Processed message {msgs[0].Values[0]}");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to process message {msgs[0].Values[0]}");
                        // TODO: retry the event
                    }
                }
                else
                {
                    Console.WriteLine("No new messages");
                    await Task.Delay(3000);
                }
            }
        }
    }
}
