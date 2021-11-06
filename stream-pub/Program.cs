using StackExchange.Redis;
using System;

namespace stream_pub
{
    class Program
    {
        private const string RedisConnectionString = "localhost:6379";
        private static readonly ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(RedisConnectionString);
        private const string StreamId = "events_stream";
        private const string ConsumerGroup = "events_consumer_group";

        static void Main(string[] args)
        {
            var db = redis.GetDatabase();
            try
            {
                db.StreamCreateConsumerGroup(StreamId, ConsumerGroup, "$");
            }
            catch (Exception)
            {
                Console.WriteLine($"{ConsumerGroup} already exists");
            }

            while (true)
            {
                for (int i=0; i<10; i++)
                {
                    var messageId = db.StreamAdd(StreamId, "message", i.ToString());
                    Console.WriteLine($"Added stream event {messageId}");
                }
                Console.WriteLine("10 events added");
                Console.ReadLine();
            }

            // TODO: clean up the stream and/or consumers and/or groups after all events have been handled
        }
    }
}
