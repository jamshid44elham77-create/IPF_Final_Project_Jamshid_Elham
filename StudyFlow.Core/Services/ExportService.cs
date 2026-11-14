using System.Globalization;
using System.Text;
using StudyFlow.Core.Models;

namespace StudyFlow.Core.Services;

public static class ExportService
{
    public static string ToCsv(IEnumerable<TaskItem> tasks, IEnumerable<Course> courses)
    {
        var names = courses.ToDictionary(c => c.Id, c => c.Name);
        var b = new StringBuilder("Title,Course,Status,Priority,Due date,Estimate (minutes),Description\r\n");
        foreach (var task in tasks)
        {
            string name = task.CourseId is Guid id && names.TryGetValue(id, out var n) ? n : "Unassigned";
            b.AppendLine(string.Join(',', new[]
            {
                Csv(task.Title), Csv(name), Csv(task.Status.ToString()), Csv(task.Priority.ToString()),
                Csv(task.DueDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? ""),
                Csv(task.EstimatedMinutes.ToString(CultureInfo.InvariantCulture)), Csv(task.Description)
            }));
        }
        return b.ToString();
    }

    public static string ToIcs(IEnumerable<TaskItem> tasks, DateTime timestampUtc)
    {
        var b = new StringBuilder("BEGIN:VCALENDAR\r\nVERSION:2.0\r\nPRODID:-//StudyFlow//Student Task Manager//EN\r\nCALSCALE:GREGORIAN\r\n");
        foreach (var task in tasks.Where(t => t.DueDate.HasValue))
        {
            b.Append("BEGIN:VTODO\r\n");
            b.Append("UID:").Append(task.Id.ToString("N")).Append("@studyflow.local\r\n");
            b.Append("DTSTAMP:").Append(timestampUtc.ToUniversalTime().ToString("yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture)).Append("\r\n");
            b.Append("DUE;VALUE=DATE:").Append(task.DueDate!.Value.ToString("yyyyMMdd", CultureInfo.InvariantCulture)).Append("\r\n");
            b.Append("SUMMARY:").Append(IcsEscape(task.Title)).Append("\r\n");
            b.Append("DESCRIPTION:").Append(IcsEscape(task.Description)).Append("\r\n");
            b.Append("STATUS:").Append(task.Status == AssignmentStatus.Completed ? "COMPLETED" : "NEEDS-ACTION").Append("\r\n");
            b.Append("END:VTODO\r\n");
        }
        return b.Append("END:VCALENDAR\r\n").ToString();
    }

    public static string Csv(string? s)
    {
        s ??= "";
        // Avoid spreadsheet formula execution when exported CSV is opened in Excel.
        if (s.Length > 0 && "=+-@\t\r".Contains(s[0])) s = "'" + s;
        return "\"" + s.Replace("\"", "\"\"") + "\"";
    }

    private static string IcsEscape(string? s) => (s ?? "")
        .Replace("\\", "\\\\").Replace(";", "\\;").Replace(",", "\\,")
        .Replace("\r\n", "\\n").Replace("\n", "\\n").Replace("\r", "\\n");
}
