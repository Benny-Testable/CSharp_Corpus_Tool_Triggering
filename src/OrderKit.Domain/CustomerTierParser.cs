namespace OrderKit;

public static class CustomerTierParser
{
    public static string Parse(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return "standard";
        }

        switch (raw.Trim().ToLowerInvariant())
        {
            case "silver":
                return "silver";
            case "gold":
                return "gold";
            case "platinum":
                return "platinum";
            default:
                return "standard";
        }
    }

    public static int DiscountPercent(string tier)
    {
        switch (Parse(tier))
        {
            case "silver":
                return 2;
            case "gold":
                return 5;
            case "platinum":
                return 10;
            default:
                return 0;
        }
    }
}
