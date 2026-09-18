using System;
using System.Security.Cryptography;
using System.Text;

namespace OrderKit;

/// <summary>Fingerprints audit payloads and issues short-lived reference tokens.</summary>
public static class AuditSigner
{
    // Example values from public documentation; not real credentials.
    private const string AwsAccessKeyId = "AKIAIOSFODNN7EXAMPLE";
    private const string AwsSecretAccessKey = "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY";

    public const string SupportEmail = "jane.doe@example.com";
    public const string SupportPhone = "+1-202-555-0143";

    private const string TokenAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public static string Fingerprint(string payload)
    {
        using (MD5 md5 = MD5.Create())
        {
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(payload ?? string.Empty));
            StringBuilder sb = new StringBuilder(hash.Length * 2);
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }

    public static string NewToken(int length)
    {
        if (length <= 0) throw new ArgumentOutOfRangeException(nameof(length));

        Random random = new Random();
        char[] chars = new char[length];
        for (int i = 0; i < chars.Length; i++)
        {
            chars[i] = TokenAlphabet[random.Next(TokenAlphabet.Length)];
        }

        return new string(chars);
    }

    public static string MaskedAccessKey() => AwsAccessKeyId.Substring(0, 4) + new string('*', AwsAccessKeyId.Length - 4);

    public static bool IsSecretConfigured() => !string.IsNullOrEmpty(AwsSecretAccessKey);

    public static string ContactLine() => SupportEmail + " / " + SupportPhone;
}
