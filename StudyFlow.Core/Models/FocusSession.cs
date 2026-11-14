namespace StudyFlow.Core.Models;

public sealed record FocusSession
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime StartedAtUtc { get; init; } = DateTime.UtcNow;
    public int DurationMinutes { get; init; }
    public Guid? TaskId { get; init; }
}
