using StudyFlow.Core.Models;

namespace StudyFlow.Core.Services;

public static class SampleData
{
    public static WorkspaceData Create(DateTime today)
    {
        var web = new Course { Name = "Web Development", Code = "WEB201", Color = "#7568F4" };
        var cs = new Course { Name = "C# & .NET", Code = "CS220", Color = "#19A7A0" };
        var math = new Course { Name = "Discrete Mathematics", Code = "MATH210", Color = "#F6A248" };
        TaskItem T(string title, Guid course, int days, TaskPriority priority, AssignmentStatus status, int minutes) =>
            new()
            {
                Title = title, CourseId = course, DueDate = today.Date.AddDays(days),
                Priority = priority, Status = status, EstimatedMinutes = minutes,
                Description = "Example assignment — edit or remove it to start with your own work.",
                CompletedAtUtc = status == AssignmentStatus.Completed ? DateTime.UtcNow : null
            };
        return new WorkspaceData
        {
            Courses = [web, cs, math],
            Tasks = [
                T("Finish Blazor final project", cs.Id, 2, TaskPriority.Urgent, AssignmentStatus.InProgress, 120),
                T("Practice C# presentation", cs.Id, 4, TaskPriority.High, AssignmentStatus.ToDo, 50),
                T("Submit responsive website", web.Id, 1, TaskPriority.High, AssignmentStatus.ToDo, 70),
                T("Review graph theory problems", math.Id, 3, TaskPriority.Medium, AssignmentStatus.ToDo, 40),
                T("Write CSS layout notes", web.Id, -1, TaskPriority.Medium, AssignmentStatus.ToDo, 30),
                T("Complete C# collections quiz", cs.Id, -3, TaskPriority.Low, AssignmentStatus.Completed, 25)
            ]
        };
    }
}
