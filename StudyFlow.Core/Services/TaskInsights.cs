using StudyFlow.Core.Models;

namespace StudyFlow.Core.Services;

public sealed record DashboardStats(int Total, int ToDo, int InProgress, int Completed,
    int Overdue, int DueSoon, int FocusMinutes)
{
    public int CompletionPercent => Total == 0 ? 0 : (int)Math.Round(100.0 * Completed / Total);
}

public static class TaskInsights
{
    public static DashboardStats Calculate(WorkspaceData workspace, DateTime today)
    {
        var tasks = workspace.Tasks;
        var active = tasks.Where(t => t.Status != AssignmentStatus.Completed).ToArray();
        return new DashboardStats(
            tasks.Count,
            tasks.Count(t => t.Status == AssignmentStatus.ToDo),
            tasks.Count(t => t.Status == AssignmentStatus.InProgress),
            tasks.Count(t => t.Status == AssignmentStatus.Completed),
            active.Count(t => t.DueDate.HasValue && t.DueDate.Value.Date < today.Date),
            active.Count(t => t.DueDate.HasValue &&
                t.DueDate.Value.Date >= today.Date && t.DueDate.Value.Date <= today.Date.AddDays(3)),
            workspace.FocusSessions.Sum(s => s.DurationMinutes));
    }

    // Deterministic, explainable next-task recommendation; no external AI service.
    public static int FocusScore(TaskItem task, DateTime today)
    {
        if (task.Status == AssignmentStatus.Completed) return int.MinValue;
        int score = ((int)task.Priority * 16) + (task.Status == AssignmentStatus.InProgress ? 9 : 0);
        if (task.DueDate.HasValue)
        {
            var days = (task.DueDate.Value.Date - today.Date).Days;
            score += days < 0 ? 80 + Math.Min(20, -days * 2) :
                days == 0 ? 75 : days <= 2 ? 60 - days * 10 : days <= 7 ? 20 : 0;
        }
        return score;
    }

    public static IReadOnlyList<TaskItem> Recommended(IEnumerable<TaskItem> tasks,
        DateTime today, int take = 3) => tasks
        .Where(t => t.Status != AssignmentStatus.Completed)
        .OrderByDescending(t => FocusScore(t, today))
        .ThenBy(t => t.DueDate ?? DateTime.MaxValue)
        .ThenBy(t => t.Title, StringComparer.OrdinalIgnoreCase)
        .Take(Math.Max(0, take)).ToList();

    public static string DueLabel(DateTime? dueDate, DateTime today)
    {
        if (dueDate is null) return "No deadline";
        int days = (dueDate.Value.Date - today.Date).Days;
        return days switch
        {
            < 0 => $"{Math.Abs(days)} day(s) overdue",
            0 => "Due today", 1 => "Due tomorrow", <= 7 => $"In {days} days",
            _ => dueDate.Value.ToString("MMM d, yyyy")
        };
    }
}
