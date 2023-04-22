using System.Text.RegularExpressions;

namespace DalleTelegramBot.Common.Extensions;

internal static class StringExtensions
{
    public static string GetCommand(this string input)
        => input.Replace(" ", "-").ToLower();

    public static string GetQuery(this string input)
        => input.Split(' ')[0].ToLower();

    public static bool GetCommand(this string input, string destination)
        => input.Replace(" ", "-").ToLower().RemoveIcons().Equals(destination);

    public static string[] GetArgs(this string input)
        => input.Split(' ').Skip(1).ToArray();

    public static string RemoveIcons(this string input)
        => Regex.Replace(input, @"[^\x00-\x7F]+|[^a-zA-Z0-9 !@#$%^&*()+={}\[\]\\|;:'"",<>\.\?/-]+", "");
}
