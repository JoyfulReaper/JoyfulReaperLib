using System.Net;

namespace JoyfulReaperLib.JRNet;

public static class IPAddressUtils
{
    /// <summary>
    /// Parses a string representation of an IP address, handling common wildcards.
    /// </summary>
    /// <param name="value">The string address format (e.g., "*", "0.0.0.0", "::", or an explicit IP).</param>
    /// <param name="libraryName">Optional library name to include in the exception message if parsing fails.</param>
    /// <returns>An <see cref="IPAddress"/> mapped to the appropriate listener configuration.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the string cannot be parsed into a valid IP address.</exception>
    public static IPAddress ParseListenAddress(string value, string? libraryName = null)
    {
        if (value is "*" or "+" or "0.0.0.0")
        {
            return IPAddress.Any;
        }

        if (value == "::")
        {
            return IPAddress.IPv6Any;
        }

        if (!IPAddress.TryParse(value, out IPAddress? address))
        {
            string prefix = !string.IsNullOrWhiteSpace(libraryName) ? $"{libraryName}: " : string.Empty;
            throw new InvalidOperationException($"{prefix}Invalid listen address: {value}.");
        }

        return address;
    }
}