namespace JoyfulReaperLib.Ntfy;

public sealed record NtfyMessage
{
    public required string Message { get; init; }

    public string? Title { get; init; }

    public NtfyPriority Priority { get; init; } = NtfyPriority.Default;

    public IReadOnlyCollection<string> Tags { get; init; } = [];

    public Uri? ClickUrl { get; init; }
}