using System.ComponentModel.DataAnnotations;

namespace StudyFlow.Core.Models;

public sealed class TaskDraft
{
    [Required(ErrorMessage = "Enter an assignment title.")]
    [StringLength(90, MinimumLength = 3, ErrorMessage = "Use 3–90 characters.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(400, ErrorMessage = "Description must be under 400 characters.")]
    public string Description { get; set; } = string.Empty;

    public Guid? CourseId { get; set; }
    public DateTime? DueDate { get; set; } = DateTime.Today.AddDays(3);
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public AssignmentStatus Status { get; set; } = AssignmentStatus.ToDo;

    [Range(5, 480, ErrorMessage = "Estimate between 5 and 480 minutes.")]
    public int EstimatedMinutes { get; set; } = 45;

    public static TaskDraft From(TaskItem task) => new()
    {
        Title = task.Title, Description = task.Description,
        CourseId = task.CourseId, DueDate = task.DueDate,
        Priority = task.Priority, Status = task.Status,
        EstimatedMinutes = task.EstimatedMinutes
    };
}
