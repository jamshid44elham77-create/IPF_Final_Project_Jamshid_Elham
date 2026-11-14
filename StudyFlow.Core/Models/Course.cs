namespace StudyFlow.Core.Models;

public sealed record Course
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string Color { get; init; } = "#6C5CE7";
}
