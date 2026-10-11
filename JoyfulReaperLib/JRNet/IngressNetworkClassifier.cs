namespace JoyfulReaperLib.JRNet;

/// <summary>
/// Classifies ingress hostnames using the DN42 and I2P suffixes and application-supplied rules.
/// </summary>
public static class IngressNetworkClassifier
{
    public const string Public = "public";
    public const string Dn42 = "dn42";
    public const string I2p = "i2p";
    public const string Yggdrasil = "yggdrasil";
    public const string Unknown = "unknown";

    /// <summary>
    /// Classifies a hostname, checking built-in DN42 and I2P suffixes, then configured
    /// Yggdrasil rules, then public rules in that order.
    /// </summary>
    /// <param name="host">A hostname only, without a scheme, port, or path.</param>
    /// <param name="publicHosts">Optional exact public hostnames.</param>
    /// <param name="publicHostSuffixes">Optional public domains and their subdomains.</param>
    /// <param name="yggdrasilHostSuffixes">Optional Yggdrasil domains and their subdomains.</param>
    /// <returns>One of <see cref="Public"/>, <see cref="Dn42"/>, <see cref="I2p"/>, <see cref="Yggdrasil"/>, or <see cref="Unknown"/>.</returns>
    /// <remarks>
    /// Hostnames and rules ignore case, surrounding whitespace, and trailing DNS dots.
    /// Suffix rules may have a leading dot; with or without it, they include the domain itself.
    /// Null or empty rule collections and null, blank, or dot-only entries are ignored.
    /// Built-in rules require a hostname below dn42 or i2p; bare dn42 and i2p do not match.
    /// This performs hostname matching only, not DNS lookup or network verification.
    /// </remarks>
    public static string FromHost(
        string? host,
        IEnumerable<string?>? publicHosts = null,
        IEnumerable<string?>? publicHostSuffixes = null,
        IEnumerable<string?>? yggdrasilHostSuffixes = null)
    {
        string hostname = Normalize(host);
        if (hostname.Length == 0)
        {
            return Unknown;
        }

        if (IsSubdomainOf(hostname, "dn42"))
        {
            return Dn42;
        }

        if (IsSubdomainOf(hostname, "i2p"))
        {
            return I2p;
        }

        if (MatchesRules(hostname, yggdrasilHostSuffixes, includeSubdomains: true))
        {
            return Yggdrasil;
        }

        if (MatchesRules(hostname, publicHosts, includeSubdomains: false)
            || MatchesRules(hostname, publicHostSuffixes, includeSubdomains: true))
        {
            return Public;
        }

        return Unknown;
    }

    private static bool MatchesRules(string host, IEnumerable<string?>? rules, bool includeSubdomains)
    {
        if (rules is null)
        {
            return false;
        }

        foreach (string? rule in rules)
        {
            string normalizedRule = Normalize(rule);
            if (includeSubdomains && normalizedRule.StartsWith('.'))
            {
                normalizedRule = normalizedRule[1..];
            }

            if (normalizedRule.Length > 0
                && (host.Equals(normalizedRule, StringComparison.OrdinalIgnoreCase)
                    || (includeSubdomains && IsSubdomainOf(host, normalizedRule))))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsSubdomainOf(string host, string suffix)
    {
        int boundary = host.Length - suffix.Length - 1;
        return boundary > 0
            && host[boundary] == '.'
            && host.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
    }

    private static string Normalize(string? value) => value?.Trim().TrimEnd('.') ?? string.Empty;
}
