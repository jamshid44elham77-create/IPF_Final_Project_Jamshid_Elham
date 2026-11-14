using System.ComponentModel.DataAnnotations;
using StudyFlow.Core.Models;

namespace StudyFlow.Core.Services;

public static class TaskRules
{
    public static OperationResult Validate(TaskDraft? draft, IEnumerable<Course> courses)
    {
        if (draft is null) return OperationResult.Fail("Task information is missing.");
        var results = new List<ValidationResult>();
        if (!Validator.TryValidateObject(draft, new ValidationContext(draft), results, true))
            return OperationResult.Fail(results[0].ErrorMessage ?? "Invalid task.");
        if (string.IsNullOrWhiteSpace(draft.Title) || draft.Title.Trim().Length < 3)
            return OperationResult.Fail("A title must contain at least three characters.");
        if (!Enum.IsDefined(draft.Priority) || !Enum.IsDefined(draft.Status))
            return OperationResult.Fail("Choose a valid priority and status.");
        if (draft.CourseId is Guid id && !courses.Any(c => c.Id == id))
            return OperationResult.Fail("The chosen course does not exist.");
        return OperationResult.Ok();
    }

    public static TaskItem Create(TaskDraft draft, DateTime nowUtc) => new()
    {
        Title = draft.Title.Trim(), Description = (draft.Description ?? "").Trim(),
        CourseId = draft.CourseId, DueDate = draft.DueDate?.Date,
        Priority = draft.Priority, Status = draft.Status,
        EstimatedMinutes = draft.EstimatedMinutes, CreatedAtUtc = nowUtc,
        UpdatedAtUtc = nowUtc,
        CompletedAtUtc = draft.Status == AssignmentStatus.Completed ? nowUtc : null
    };

    public static TaskItem Update(TaskItem source, TaskDraft draft, DateTime nowUtc) => source with
    {
        Title = draft.Title.Trim(), Description = (draft.Description ?? "").Trim(),
        CourseId = draft.CourseId, DueDate = draft.DueDate?.Date,
        Priority = draft.Priority, Status = draft.Status,
        EstimatedMinutes = draft.EstimatedMinutes, UpdatedAtUtc = nowUtc,
        CompletedAtUtc = draft.Status == AssignmentStatus.Completed
            ? source.CompletedAtUtc ?? nowUtc : null
    };

    public static TaskItem SetStatus(TaskItem source, AssignmentStatus status, DateTime nowUtc) =>
        source with
        {
            Status = status, UpdatedAtUtc = nowUtc,
            CompletedAtUtc = status == AssignmentStatus.Completed ? source.CompletedAtUtc ?? nowUtc : null
        };
}
