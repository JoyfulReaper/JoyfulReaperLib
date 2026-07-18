namespace JoyfulReaperLib.Ntfy;

/*
 {
  "Ntfy": {
    "ServerUrl": "https://ntfy.kgivler.com",
    "Topic": "mission-control",
    "AccessToken": "tk_replace_me",
    "Timeout": "00:00:10"
  }
}
*/

public sealed class NtfyOptions
{
    public const string SectionName = "Ntfy";

    public required Uri ServerUrl { get; set; }

    public required string Topic { get; set; }

    public string? AccessToken { get; set; }

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
}