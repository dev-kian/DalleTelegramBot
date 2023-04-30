using DalleTelegramBot.Common.Caching.SharedData;
using Microsoft.Extensions.Caching.Memory;

namespace DalleTelegramBot.Common.Caching
{
    internal class StateManagementMemoryCache
    {
        private readonly IMemoryCache _cache;
        public StateManagementMemoryCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void SetLastCommand(long userId, string lastCommand, byte state = 1, params object[] data)
        {
            if(_cache.TryGetValue(userId, out UserMessageInfo? userMessageInfo))
            {
                userMessageInfo.LastCommand = lastCommand;
                userMessageInfo.StateCommand = state;
                userMessageInfo.Data = data;
            }
            else
            {
                userMessageInfo = new() { LastCommand = lastCommand, StateCommand = state, Data = data };
                _cache.Set(userId, userMessageInfo, DateTimeOffset.UtcNow.AddDays(1));
            }
        }

        public bool CanGetLastCommand(long userId, string lastCommand, byte state, bool deleteCommand = false)
        {
            bool existsMessageInfo = _cache.TryGetValue(userId, out UserMessageInfo? userMessageInfo);
            if (!existsMessageInfo || userMessageInfo is null)
                return false;

            if(!string.IsNullOrEmpty(userMessageInfo.LastCommand) && userMessageInfo.LastCommand.Equals(lastCommand) && userMessageInfo.StateCommand == state)
            {
                if (deleteCommand)
                    ClearLastCommand(userMessageInfo!);

                return true;
            }

            return false;
        }

        public bool CanGetLastCommand(long userId, out string lastCommand)
        {
            bool existsMessageInfo = _cache.TryGetValue(userId, out UserMessageInfo? userMessageInfo);
            lastCommand = (existsMessageInfo && !string.IsNullOrEmpty(userMessageInfo!.LastCommand)) ? userMessageInfo!.LastCommand : null!;
            return lastCommand != null!;
        }

        public object[] CanGetCommandData(long userId, bool deleteCommand = false)
        {
            bool existsMessageInfo = _cache.TryGetValue(userId, out UserMessageInfo? userMessageInfo);
            var data = (existsMessageInfo ? userMessageInfo?.Data : null)!;
            if (existsMessageInfo && deleteCommand)
                ClearLastCommand(userMessageInfo!);
            return data;
        }

        public void AddDataIfExistsCommand(long userId, params object[] data)
        {
            bool existsMessageInfo = _cache.TryGetValue(userId, out UserMessageInfo? userMessageInfo);
            if (existsMessageInfo && userMessageInfo!.Data is not null)
            {
                userMessageInfo.Data = userMessageInfo!.Data.Concat(data).ToArray();
            }
        }

        public void RemoveLastCommand(long userId)
        {
            bool existsMessageInfo = _cache.TryGetValue(userId, out UserMessageInfo? userMessageInfo);
            if (existsMessageInfo)
                ClearLastCommand(userMessageInfo!);
        }

        private void ClearLastCommand(UserMessageInfo userMessageInfo)
        {
            userMessageInfo!.LastCommand = null!;
            userMessageInfo!.StateCommand = 0;
            userMessageInfo!.Data = null!;
        }
    }
}
