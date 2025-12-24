using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Conector4.General
{
    public class QueueCounters
    {
        public long Processed;
        public long Failed;
        public long InFlight;
    }

    public static class Metrics
    {
        private static readonly ConcurrentDictionary<string, QueueCounters> _map = new();

        private static QueueCounters GetOrCreate(string queue)
        {
            return _map.GetOrAdd(queue, _ => new QueueCounters());
        }

        public static void IncrementProcessed(string queue)
        {
            var c = GetOrCreate(queue);
            Interlocked.Increment(ref c.Processed);
        }

        public static void IncrementFailed(string queue)
        {
            var c = GetOrCreate(queue);
            Interlocked.Increment(ref c.Failed);
        }

        public static void IncrementInFlight(string queue)
        {
            var c = GetOrCreate(queue);
            Interlocked.Increment(ref c.InFlight);
        }

        public static void DecrementInFlight(string queue)
        {
            var c = GetOrCreate(queue);
            Interlocked.Decrement(ref c.InFlight);
        }

        public static Dictionary<string, QueueCounters> Snapshot()
        {
            var dict = new Dictionary<string, QueueCounters>();
            foreach (var kv in _map)
            {
                var copy = new QueueCounters
                {
                    Processed = Interlocked.Read(ref kv.Value.Processed),
                    Failed = Interlocked.Read(ref kv.Value.Failed),
                    InFlight = Interlocked.Read(ref kv.Value.InFlight)
                };
                dict[kv.Key] = copy;
            }
            return dict;
        }
    }
}
