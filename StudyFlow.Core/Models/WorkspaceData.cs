namespace StudyFlow.Core.Models;

public sealed class WorkspaceData
{
    public List<Course> Courses { get; set; } = [];
    public List<TaskItem> Tasks { get; set; } = [];
    public List<FocusSession> FocusSessions { get; set; } = [];

    public WorkspaceData Copy() => new()
    {
        Courses = Courses.ToList(),
        Tasks = Tasks.ToList(),
        FocusSessions = FocusSessions.ToList()
    };
}
