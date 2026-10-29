using System;
using System.Collections.Generic;

namespace OrderKit;

/// <summary>
/// Scores an order from 0 to 100. Hazmat and fragile lines add points, watched
/// regions multiply the hazmat exposure, and hard or soft flags add a fixed amount.
/// </summary>
public sealed class RiskScorer
{
    private const int MaxScore = 100;
    private const int HazmatThreshold = 10;
    private const int FragileThreshold = 50;
    private const int BulkQuantity = 1_000;

    private readonly ISet<string> _watchedRegions =
        new HashSet<string> { "sanctioned", "embargoed" };

    public int Score(Order order, string region, IList<string> flags)
    {
        int score = LineScore(order, region) + FlagScore(flags);
        return Math.Min(score, MaxScore);
    }

    /// <summary>Exception filters let the catch decline without unwinding.</summary>
    public int ScoreOrDefault(Order order, string region, IList<string> flags)
    {
        try
        {
            return Score(order, region, flags);
        }
        catch (ArgumentException ex) when (ex.ParamName == "region")
        {
            return 0;
        }
    }

    public bool IsHighRisk(Order order, string region, IList<string> flags) =>
        Score(order, region, flags) >= 60;

    public int ScoreAny(IEnumerable<object> items)
    {
        int score = 0;
        foreach (object item in items ?? new object[0])
        {
            if (item is Order order)
            {
                score += Score(order, null, null);
            }
            else if (item is int raw)
            {
                score += raw;
            }
        }

        return Math.Min(score, MaxScore);
    }

    private int LineScore(Order order, string region)
    {
        int score = 0;
        if (order != null)
        {
            if (!order.IsEmpty())
            {
                foreach (OrderLine line in order.Lines)
                {
                    if (line.Category == "hazmat")
                    {
                        if (line.Quantity > HazmatThreshold)
                        {
                            if (region != null)
                            {
                                if (_watchedRegions.Contains(region.ToLowerInvariant()))
                                {
                                    score += 40;
                                }
                                else
                                {
                                    score += 15;
                                }
                            }
                            else
                            {
                                score += 25;
                            }
                        }
                        else
                        {
                            score += 5;
                        }
                    }
                    else if (line.Category == "fragile")
                    {
                        if (line.Quantity > FragileThreshold)
                        {
                            score += line.Quantity > BulkQuantity ? 12 : 8;
                        }
                    }
                }
            }
        }

        return score;
    }

    private static int FlagScore(IList<string> flags)
    {
        int score = 0;
        if (flags != null)
        {
            foreach (string flag in flags)
            {
                if (flag != null)
                {
                    if (flag.StartsWith("hard-", StringComparison.Ordinal))
                    {
                        score += 20;
                    }
                    else if (flag.StartsWith("soft-", StringComparison.Ordinal))
                    {
                        score += 3;
                    }
                }
            }
        }

        return score;
    }
}
