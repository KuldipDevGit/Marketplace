using System.Text;

namespace Marketplace.Catalog.Application.Common;

/// <summary>Generates URL-safe slugs from free text (lowercase, alphanumeric, hyphen-separated).</summary>
public static class Slug
{
    public static string From(string input)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(input);

        var builder = new StringBuilder(input.Length);
        foreach (var ch in input.Trim().ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(ch))
            {
                builder.Append(ch);
            }
            else if (ch is ' ' or '-' or '_' or '.')
            {
                builder.Append('-');
            }
        }

        var slug = builder.ToString();
        while (slug.Contains("--", StringComparison.Ordinal))
        {
            slug = slug.Replace("--", "-", StringComparison.Ordinal);
        }

        return slug.Trim('-');
    }
}
