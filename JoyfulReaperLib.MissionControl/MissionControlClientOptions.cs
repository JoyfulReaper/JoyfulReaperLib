namespace JoyfulReaperLib.MissionControl;

public sealed class MissionControlClientOptions
{
    public const string SectionName = "MissionControl";

    internal const string HttpClientName =
        "JoyfulReaperLib.MissionControl";

    internal const string ApiKeyHeaderName =
        "X-Mission-Control-Key";

    public bool Enabled { get; set; }

    public string BaseUrl { get; set; } =
        "http://localhost:5190";

    public string ApiKey { get; set; } =
        string.Empty;

    public int TimeoutMilliseconds { get; set; } =
        1000;
}