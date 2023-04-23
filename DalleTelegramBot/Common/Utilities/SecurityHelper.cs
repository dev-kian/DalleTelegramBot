using System.Security.Cryptography;

namespace DalleTelegramBot.Common.Utilities;

internal class SecurityHelper
{
    public static async Task<string> GetFileHash(string filePath)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] hashBytes = await sha256.ComputeHashAsync(fileStream);
                string hashString = BitConverter.ToString(hashBytes).Replace("-", "");
                return hashString;
            }
        }
    }
}
