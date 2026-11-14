namespace StudyFlow.Core.Models;

public enum TaskPriority { Low = 1, Medium = 2, High = 3, Urgent = 4 }
public enum AssignmentStatus { ToDo = 0, InProgress = 1, Completed = 2 }

public sealed record TaskItem
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Guid? CourseId { get; init; }
    public DateTime? DueDate { get; init; }
    public TaskPriority Priority { get; init; } = TaskPriority.Medium;
    public AssignmentStatus Status { get; init; } = AssignmentStatus.ToDo;
    public int EstimatedMinutes { get; init; } = 45;
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; init; } = DateTime.UtcNow;
    public DateTime? CompletedAtUtc { get; init; }
}
