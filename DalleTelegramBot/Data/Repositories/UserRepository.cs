using DalleTelegramBot.Data.Contracts;
using DalleTelegramBot.Data.Models;
using Dapper;
using System.Data.SQLite;

namespace DalleTelegramBot.Data.Repositories
{
    internal class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        public UserRepository(string connectionString) =>
            _connectionString=connectionString;


        public async Task<int> AddAsync(User user)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                const string Sql = "INSERT INTO Users (Id, IsBan, CreateTime, ImageSize, ImageCount) VALUES (@Id, @IsBan, @CreateTime, @ImageSize, @ImageCount)";
                return await connection.ExecuteAsync(Sql, user);
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            using(var connection = new SQLiteConnection(_connectionString))
            {
                const string Sql = "SELECT * FROM Users";
                return await connection.QueryAsync<User>(Sql);
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync(bool isBan)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                const string Sql = "SELECT * FROM Users WHERE IsBan = @IsBan";
                return await connection.QueryAsync<User>(Sql, new { IsBan = isBan });
            }
        }

        public async Task<User> GetByIdAsync(long id)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                const string Sql = "SELECT * FROM Users WHERE Id = @Id";
                return await connection.QuerySingleOrDefaultAsync<User>(Sql, new { Id = id });
            }
        }

        public async Task<bool> AnyAsync(long id)
        {

            using (var connection = new SQLiteConnection(_connectionString))
            {
                const string Sql = "SELECT COUNT(*) FROM Users WHERE Id = @Id";
                var result = await connection.ExecuteScalarAsync<int>(Sql, new { Id = id });
                return result > 0;
            }
        }

        public async Task<int> GetCountAsync()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                const string Sql = "SELECT COUNT(*) FROM Users";
                return await connection.QuerySingleAsync<int>(Sql);
            }
        }

        public async Task<int> GetCountAsync(bool isBan)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                const string Sql = "SELECT COUNT(*) FROM Users WHERE IsBan = @IsBan";
                return await connection.QuerySingleAsync<int>(Sql, new {IsBan = isBan});
            }
        }

        public async Task<int> UpdateBanStateAsync(long id, bool isBan)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                const string Sql = "UPDATE Users SET IsBan = @IsBan WHERE Id = @Id";
                var parameter = new { Id = id, IsBan = isBan };
                var result = await connection.ExecuteAsync(Sql, parameter);
                return result;
            }
        }

        public async Task<int> UpdateImageSizeAsync(long id, int imageSize)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                const string Sql = "UPDATE Users SET ImageSize = @ImageSize WHERE Id = @Id";
                var parameter = new { Id = id, ImageSize = imageSize };
                var result = await connection.ExecuteAsync(Sql, parameter);
                return result;
            }
        }

        public async Task<int> UpdateImageCountAsync(long id, int imageCount)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                const string Sql = "UPDATE Users SET ImageCount = @ImageCount WHERE Id = @Id";
                var parameter = new { Id = id, ImageCount = imageCount };
                var result = await connection.ExecuteAsync(Sql, parameter);
                return result;
            }
        }
    
        public async Task<int> UpdateApiKeyAsync(long id, string? apiKey= null!)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                const string Sql = "UPDATE Users SET ApiKey = @ApiKey WHERE Id = @Id";
                var parameter = new { Id = id, ApiKey = apiKey };
                var result = await connection.ExecuteAsync(Sql, parameter);
                return result;
            }
        }
    }
}
