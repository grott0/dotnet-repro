using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics.Tracing;

namespace MemCacheCounter
{
    [EventSource(Name = "Microsoft-Extensions-Caching-Memory")]
    internal sealed class CachingEventSource : EventSource
    {
        private readonly IMemoryCache _memoryCache;
        private PollingCounter _cacheHitsCounter;

        public CachingEventSource(IMemoryCache memoryCache) { _memoryCache = memoryCache; }
        protected override void OnEventCommand(EventCommandEventArgs command)
        {
            if (command.Command == EventCommand.Enable)
            {
                if (_cacheHitsCounter == null)
                {
                    _cacheHitsCounter = new PollingCounter("cache-hits", this, () =>
                        _memoryCache.GetCurrentStatistics().TotalHits)
                    {
                        DisplayName = "Cache hits",
                    };
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            _cacheHitsCounter.Dispose();
            _cacheHitsCounter = null;

            base.Dispose(disposing);
        }
    }
}
