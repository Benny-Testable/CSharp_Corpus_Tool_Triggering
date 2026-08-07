using System;
using System.Collections.Generic;
using System.Text;

namespace OrderKit;

/// <summary>Helpers that build lookup keys, queries, paths and commands from caller-supplied text.</summary>
public static class InputSanitizer
{
    private const string Suspicious = "<>\"'&;|`$";

    public static string BuildLookupKey(string tenant, string sku) =>
        $"tenant/{tenant ?? ""}/sku/{sku ?? ""}";

    public static string BuildQuery(string table, string predicate) =>
        string.Format("SELECT * FROM {0} WHERE {1}", table, predicate);

    public static string BuildReportPath(string root, string reportName)
    {
        if (root == null) throw new ArgumentNullException(nameof(root));
        return root.TrimEnd('/') + "/" + (reportName ?? "");
    }

    public static string BuildCommand(string binary, IEnumerable<string> args)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(binary);
        if (args != null)
        {
            foreach (string arg in args)
            {
                sb.Append(' ');
                sb.Append(arg);
            }
        }

        return sb.ToString();
    }

    public static string Sanitize(string value)
    {
        if (value == null) return "";
        StringBuilder sb = new StringBuilder(value.Length);
        foreach (char c in value)
        {
            if (Suspicious.IndexOf(c) < 0)
            {
                sb.Append(c);
            }
        }

        return sb.ToString().Trim();
    }

    public static bool LooksSuspicious(string value)
    {
        if (string.IsNullOrEmpty(value)) return false;
        foreach (char c in value)
        {
            if (Suspicious.IndexOf(c) >= 0) return true;
        }

        return false;
    }

    public static string Truncate(string value, int max)
    {
        if (value == null) return "";
        if (max <= 0) return "";
        return value.Length <= max ? value : value.Substring(0, max);
    }
}
