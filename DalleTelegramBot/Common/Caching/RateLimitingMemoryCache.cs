using DalleTelegramBot.Common.Caching.SharedData;
using DalleTelegramBot.Configurations;
using Microsoft.Extensions.Caching.Memory;

namespace DalleTelegramBot.Common.Caching
{
    internal class RateLimitingMemoryCache
    {
        private readonly IMemoryCache _cache;
        public RateLimitingMemoryCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public bool IsMessageLimitExceeded(long userId)
        {
            if (_cache.TryGetValue(userId, out UserMessageInfo? userMessageInfo))
            {
                if (userMessageInfo?.MessageCount >= BotConfig.RateLimitCount)
                {
                    if ((DateTime.UtcNow - userMessageInfo.LastMessageTime) < TimeSpan.FromDays(1))
                    {
                        return true;
                    }
                    else
                    {
                        _cache.Remove(userId);
                    }
                }
            }
            return false;
        }

        public void UpdateUserMessageCount(long userId, int msgCount = 1)
        {
            if (_cache.TryGetValue(userId, out UserMessageInfo? userMessageInfo))
            {
                userMessageInfo.MessageCount += msgCount;
                userMessageInfo.LastMessageTime = DateTime.UtcNow;
                //_cache.Set(userId, userMessageInfo, DateTimeOffset.UtcNow.AddDays(1));
            }
            else
            {
                userMessageInfo = new() { MessageCount = msgCount, LastMessageTime = DateTime.UtcNow };
                _cache.Set(userId, userMessageInfo, DateTimeOffset.UtcNow.AddDays(1));
            }
        }

        public int GetMessageCount(long userId)
        {
            if (_cache.TryGetValue(userId, out UserMessageInfo? userMessageInfo))
                return userMessageInfo.MessageCount;

            return 0;
        }
    }
}
