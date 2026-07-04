namespace JoyfulReaperLib.WebStats.Sqlite;

public interface IHitCounter
{
    Task<HitCountStats> GetHitCountsAsync(CancellationToken ct = default);

    Task<HitCountStats> RecordHitAsync(string visitorKey, CancellationToken ct = default);
}
