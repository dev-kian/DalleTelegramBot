using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data.SQLite;

namespace DalleTelegramBot.Data.Repositories
{
    internal class DatabaseInitializer
    {
        public static async Task InitializeAsync(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("CS")!;
            string dbPath = Path.Combine(Environment.CurrentDirectory, connectionString.Split('=')[1]);
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);

                using(var connection = new SQLiteConnection(connectionString))
                {
                    const string Sql = @"CREATE TABLE ""Users"" (""Id""  INTEGER NOT NULL UNIQUE,""IsBan""  INTEGER,""CreateTime""  DATETIME,""ApiKey""  TEXT,""ImageSize""  INTEGER,""ImageCount""  INTEGER,PRIMARY KEY(""Id""));";
                    await connection.ExecuteAsync(Sql);
                }
            }
        }
    }
}
