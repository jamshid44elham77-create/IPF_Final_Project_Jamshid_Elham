using CoreStatus = StudyFlow.Core.Models.AssignmentStatus;
using StudyFlow.Core.Models;
using StudyFlow.Core.Services;
using StudyFlow.Web.Services;

// Dependency-free automated regression runner. Run: dotnet run --project StudyFlow.Tests
var testCases = new (string Name, Func<Task> Run)[]
{
    ("Title validation rejects blank", () => { Expect(!TaskRules.Validate(new TaskDraft { Title = "   " }, []).Success); return Task.CompletedTask; }),
    ("Title validation accepts valid", () => { Expect(TaskRules.Validate(new TaskDraft { Title = "Write report" }, []).Success); return Task.CompletedTask; }),
    ("Too-long description rejected", () => { Expect(!TaskRules.Validate(new TaskDraft { Title = "Report", Description = new string('x', 401) }, []).Success); return Task.CompletedTask; }),
    ("Nonexistent course rejected", () => { Expect(!TaskRules.Validate(new TaskDraft { Title = "Report", CourseId = Guid.NewGuid() }, []).Success); return Task.CompletedTask; }),
    ("Time estimate validated", () => { Expect(!TaskRules.Validate(new TaskDraft { Title = "Report", EstimatedMinutes = 999 }, []).Success); return Task.CompletedTask; }),
    ("Task creation trims title", () => { Expect(TaskRules.Create(new TaskDraft { Title = "  My task  " }, DateTime.UtcNow).Title == "My task"); return Task.CompletedTask; }),
    ("Completing task sets completion date", () => { Expect(TaskRules.SetStatus(new TaskItem(), CoreStatus.Completed, DateTime.UtcNow).CompletedAtUtc.HasValue); return Task.CompletedTask; }),
    ("Reopening clears completion date", () => { Expect(TaskRules.SetStatus(new TaskItem { Status = CoreStatus.Completed, CompletedAtUtc = DateTime.UtcNow }, CoreStatus.ToDo, DateTime.UtcNow).CompletedAtUtc is null); return Task.CompletedTask; }),
    ("Overdue assignments ranked above later ones", () => { var today = DateTime.Today; var a = new TaskItem { DueDate = today.AddDays(-2), Priority = TaskPriority.Medium }; var b = new TaskItem { DueDate = today.AddDays(12), Priority = TaskPriority.Medium }; Expect(TaskInsights.FocusScore(a, today) > TaskInsights.FocusScore(b, today)); return Task.CompletedTask; }),
    ("Recommendations omit completed tasks", () => { Expect(TaskInsights.Recommended([new TaskItem { Status = CoreStatus.Completed }], DateTime.Today).Count == 0); return Task.CompletedTask; }),
    ("Dashboard handles empty tasks", () => { var stats = TaskInsights.Calculate(new WorkspaceData(), DateTime.Today); Expect(stats.CompletionPercent == 0 && stats.Total == 0); return Task.CompletedTask; }),
    ("Dashboard reports overdue correctly", () => { var stats = TaskInsights.Calculate(new WorkspaceData { Tasks = [new TaskItem { DueDate = DateTime.Today.AddDays(-1) }] }, DateTime.Today); Expect(stats.Overdue == 1); return Task.CompletedTask; }),
    ("CSV prevents formula injection", () => { Expect(ExportService.Csv("=1+1").StartsWith("\"'=1+1")); return Task.CompletedTask; }),
    ("CSV escapes embedded quotes", () => { Expect(ExportService.Csv("A\"B") == "\"A\"\"B\""); return Task.CompletedTask; }),
    ("iCalendar includes due task", () => { var text = ExportService.ToIcs([new TaskItem { Title = "Deadline", DueDate = DateTime.Today }], DateTime.UtcNow); Expect(text.Contains("BEGIN:VTODO") && text.Contains("SUMMARY:Deadline") && text.Contains("END:VCALENDAR")); return Task.CompletedTask; }),
    ("iCalendar excludes undated task", () => { var text = ExportService.ToIcs([new TaskItem { Title = "No date" }], DateTime.UtcNow); Expect(!text.Contains("BEGIN:VTODO")); return Task.CompletedTask; }),
    ("JSON repository stores and reloads data", async () =>
    {
        var path = NewTempPath(); try
        {
            var repo = new JsonWorkspaceRepository(path, seedIfMissing: false);
            var data = await repo.ReadAsync(); Expect(data.Tasks.Count == 0);
            data.Tasks.Add(new TaskItem { Title = "Persisted item" });
            await repo.SaveAsync(data);
            var reloaded = await new JsonWorkspaceRepository(path, false).ReadAsync();
            Expect(reloaded.Tasks.Single().Title == "Persisted item");
        }
        finally { DeleteTemp(path); }
    }),
    ("Service CRUD integration", async () =>
    {
        var path = NewTempPath(); try
        {
            var service = new WorkspaceService(new JsonWorkspaceRepository(path, false));
            await service.InitializeAsync();
            Expect((await service.CreateTaskAsync(new TaskDraft { Title = "First report" })).Success);
            var id = service.Tasks.Single().Id;
            Expect((await service.UpdateTaskAsync(id, new TaskDraft { Title = "Revised report", Priority = TaskPriority.Urgent })).Success);
            Expect(service.Tasks.Single().Priority == TaskPriority.Urgent);
            Expect((await service.SetAssignmentStatusAsync(id, CoreStatus.Completed)).Success);
            Expect(service.Stats.Completed == 1);
            Expect((await service.DeleteTaskAsync(id)).Success);
            Expect(service.Tasks.Count == 0);
        }
        finally { DeleteTemp(path); }
    }),
    ("Course deletion protects related tasks", async () =>
    {
        var path = NewTempPath(); try
        {
            var service = new WorkspaceService(new JsonWorkspaceRepository(path, false));
            await service.InitializeAsync();
            Expect((await service.AddCourseAsync("Math", "MATH1", "#F6A248")).Success);
            var id = service.Courses.Single().Id;
            Expect((await service.CreateTaskAsync(new TaskDraft { Title = "Math exercise", CourseId = id })).Success);
            Expect(!(await service.DeleteCourseAsync(id)).Success);
        }
        finally { DeleteTemp(path); }
    }),
    ("Focus sessions persist and update totals", async () =>
    {
        var path = NewTempPath(); try
        {
            var service = new WorkspaceService(new JsonWorkspaceRepository(path, false));
            await service.InitializeAsync();
            Expect((await service.AddFocusSessionAsync(DateTime.UtcNow, 25, null)).Success);
            Expect(service.Stats.FocusMinutes == 25);
            Expect(!(await service.AddFocusSessionAsync(DateTime.UtcNow, 0, null)).Success);
        }
        finally { DeleteTemp(path); }
    })
};

var passed = 0;
foreach (var test in testCases)
{
    try { await test.Run(); Console.WriteLine($"PASS  {test.Name}"); passed++; }
    catch (Exception ex) { Console.WriteLine($"FAIL  {test.Name}\n      {ex.Message}"); }
}
Console.WriteLine($"\nRESULT: {passed}/{testCases.Length} tests passed");
return passed == testCases.Length ? 0 : 1;

static void Expect(bool condition)
{
    if (!condition) throw new Exception("Assertion failed");
}
static string NewTempPath() => Path.Combine(Path.GetTempPath(), "studyflow-test-" + Guid.NewGuid().ToString("N"), "workspace.json");
static void DeleteTemp(string path)
{
    var dir = Path.GetDirectoryName(path);
    if (dir is not null && Directory.Exists(dir)) Directory.Delete(dir, true);
}

