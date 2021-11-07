using StackExchange.Redis;
using stream_shared.Extensions;
using System;
using System.Linq;
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
            var consumerId = Guid.NewGuid().ToString();
            Console.WriteLine($"Consumer {consumerId} starting...");
            var db = redis.GetDatabase();

            while (true)
            {
                var msgs = await db.StreamReadGroupAsync(StreamId, ConsumerGroup, consumerId, ">", count: 1);

                if (msgs.Length > 0)
                {
                    var msg = msgs[0];
                    Console.Write($"Received message {msg.Id} with value {msg.Values[0]}...");
                    await ProcessMessage(db, msg);
                }
                else
                {
                    Console.WriteLine("No new messages");

                    // Get pending messages for all consumers
                    var pendingMessages = await db.StreamPendingMessagesAsync(StreamId, ConsumerGroup, count: 1, RedisValue.Null);

                    if (pendingMessages.Length > 0)
                    {
                        // Claim the first pending message that appears to have failed due to age
                        var retryMessages = await db.StreamClaimAsync(StreamId,
                            ConsumerGroup,
                            consumerId,
                            minIdleTimeInMs: 1000 * 10,
                            messageIds: pendingMessages.Select(pm => pm.MessageId).ToArray());

                        if (retryMessages.Length > 0)
                        {
                            var msg = retryMessages[0];
                            ConsoleColor.Blue.Write($"Retrying message {msg.Id} with value {msg.Values[0]}...");
                            await ProcessMessage(db, msg);
                        }
                    }

                    await Task.Delay(3000);
                }
            }
        }

        private static async Task ProcessMessage(IDatabase db, StreamEntry msg)
        {
            // Simulate occasional consumer failures
            Random rand = new();
            await Task.Delay(3000);
            if (rand.Next(0, 10) > 3)
            {
                ConsoleColor.Green.WriteLine("Processed message");

                // Acknowledge the message, removing it from pending
                await db.StreamAcknowledgeAsync(StreamId, ConsumerGroup, msg.Id);
            }
            else
            {
                ConsoleColor.Red.WriteLine("Failed!");
            }
        }
    }
}
