using System.Text.RegularExpressions;

namespace OrderKit;

/// <summary>SKUs look like ABC-1234: three capital letters, a dash and four digits.</summary>
public static class SkuFormat
{
    private static readonly Regex Pattern = new Regex(@"[A-Z]{3}-\d{4}");

    public static bool IsValid(string sku) => !string.IsNullOrEmpty(sku) && Pattern.IsMatch(sku);

    public static string Normalize(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return string.Empty;
        }

        return raw.Trim().ToUpperInvariant().Replace(' ', '-');
    }
}
