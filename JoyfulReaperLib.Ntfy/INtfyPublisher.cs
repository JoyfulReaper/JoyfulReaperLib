namespace JoyfulReaperLib.Ntfy;

public interface INtfyPublisher
{
    Task PublishAsync(
        NtfyMessage notification,
        CancellationToken cancellationToken = default);
}