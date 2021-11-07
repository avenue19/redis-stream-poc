using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace stream_pub
{
    class Program
    {
        private const string RedisConnectionString = "localhost:6379";
        private static readonly ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(RedisConnectionString);
        private const string StreamId = "events_stream";
        private const string ConsumerGroup = "events_consumer_group";

        static async Task Main(string[] args)
        {
            var db = redis.GetDatabase();
            if (! await db.KeyExistsAsync(StreamId))
            {
                if (! await db.StreamCreateConsumerGroupAsync(StreamId, ConsumerGroup, "$"))
                {
                    throw new ApplicationException("Could not create consumer group");
                }
            }

            while (true)
            {
                for (int i=0; i<10; i++)
                {
                    var messageId = await db.StreamAddAsync(StreamId, "message", i.ToString());
                    Console.WriteLine($"Added stream event {messageId}");
                }
                Console.WriteLine("10 events added");
                Console.ReadLine();
            }

            // TODO: clean up the stream and/or consumers and/or groups after all events have been handled
        }
    }
}
