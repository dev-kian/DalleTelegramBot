using DalleTelegramBot.Data.Models;

namespace DalleTelegramBot.Data.Contracts;

internal interface IUserRepository
{
    Task<int> AddAsync(User user);
    Task<IEnumerable<User>> GetAllAsync();
    Task<IEnumerable<User>> GetAllAsync(bool isBan);
    Task<User> GetByIdAsync(long id);
    Task<bool> AnyAsync(long id);
    Task<int> GetCountAsync();
    Task<int> GetCountAsync(bool isBan);
    Task<int> UpdateBanStateAsync(long id, bool isBan);
    Task<int> UpdateImageSizeAsync(long id, int imageSize);
    Task<int> UpdateImageCountAsync(long id, int imageCount);
    Task<int> UpdateApiKeyAsync(long id, string? apiKey = null!);
}
