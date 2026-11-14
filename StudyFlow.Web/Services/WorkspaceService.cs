using StudyFlow.Core.Models;
using StudyFlow.Core.Services;

namespace StudyFlow.Web.Services;

// Scoped state container: Blazor components use the same snapshot and Changed event per circuit.
public sealed class WorkspaceService
{
    private readonly IWorkspaceRepository _repository;
    private readonly SemaphoreSlim _writeGate = new(1, 1);
    private WorkspaceData _data = new();
    private bool _initialized;

    public WorkspaceService(IWorkspaceRepository repository) => _repository = repository;
    public event Action? Changed;
    public string? LoadError { get; private set; }
    public bool IsLoaded => _initialized;
    public IReadOnlyList<TaskItem> Tasks => _data.Tasks;
    public IReadOnlyList<Course> Courses => _data.Courses;
    public IReadOnlyList<FocusSession> Sessions => _data.FocusSessions;
    public DashboardStats Stats => TaskInsights.Calculate(_data, DateTime.Today);
    public IReadOnlyList<TaskItem> SuggestedTasks => TaskInsights.Recommended(_data.Tasks, DateTime.Today);
    public Course? FindCourse(Guid? id) => id.HasValue ? _data.Courses.FirstOrDefault(c => c.Id == id) : null;

    public async Task InitializeAsync()
    {
        if (_initialized) return;
        await _writeGate.WaitAsync();
        try
        {
            if (_initialized) return;
            _data = await _repository.ReadAsync();
            _initialized = true;
            LoadError = null;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Text.Json.JsonException)
        {
            LoadError = "Could not open the saved workspace: " + ex.Message;
        }
        finally { _writeGate.Release(); }
        Changed?.Invoke();
    }

    public Task<OperationResult> CreateTaskAsync(TaskDraft draft) => ApplyAsync(data =>
    {
        var validation = TaskRules.Validate(draft, data.Courses);
        if (!validation.Success) return validation;
        data.Tasks.Insert(0, TaskRules.Create(draft, DateTime.UtcNow));
        return OperationResult.Ok();
    });

    public Task<OperationResult> UpdateTaskAsync(Guid id, TaskDraft draft) => ApplyAsync(data =>
    {
        var validation = TaskRules.Validate(draft, data.Courses);
        if (!validation.Success) return validation;
        var index = data.Tasks.FindIndex(t => t.Id == id);
        if (index < 0) return OperationResult.Fail("That task no longer exists.");
        data.Tasks[index] = TaskRules.Update(data.Tasks[index], draft, DateTime.UtcNow);
        return OperationResult.Ok();
    });

    public Task<OperationResult> DeleteTaskAsync(Guid id) => ApplyAsync(data =>
    {
        if (data.Tasks.RemoveAll(t => t.Id == id) == 0)
            return OperationResult.Fail("Task was not found.");
        return OperationResult.Ok();
    });

    public Task<OperationResult> SetAssignmentStatusAsync(Guid id, AssignmentStatus status) => ApplyAsync(data =>
    {
        if (!Enum.IsDefined(status)) return OperationResult.Fail("Invalid task status.");
        var index = data.Tasks.FindIndex(t => t.Id == id);
        if (index < 0) return OperationResult.Fail("Task was not found.");
        data.Tasks[index] = TaskRules.SetStatus(data.Tasks[index], status, DateTime.UtcNow);
        return OperationResult.Ok();
    });

    public Task<OperationResult> AddCourseAsync(string name, string code, string color) => ApplyAsync(data =>
    {
        name = name.Trim(); code = code.Trim().ToUpperInvariant();
        if (name.Length is < 2 or > 55 || code.Length is < 2 or > 15)
            return OperationResult.Fail("Course name must be 2–55 characters and code 2–15 characters.");
        if (data.Courses.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) ||
            c.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
            return OperationResult.Fail("A course with that name or code already exists.");
        if (!new[] { "#7568F4", "#19A7A0", "#F6A248", "#E879A7", "#438FD0" }.Contains(color))
            return OperationResult.Fail("Choose one of the available course colors.");
        data.Courses.Add(new Course { Name = name, Code = code, Color = color });
        return OperationResult.Ok();
    });

    public Task<OperationResult> DeleteCourseAsync(Guid id) => ApplyAsync(data =>
    {
        if (data.Tasks.Any(t => t.CourseId == id))
            return OperationResult.Fail("Move or delete assignments in this course first.");
        if (data.Courses.RemoveAll(c => c.Id == id) == 0)
            return OperationResult.Fail("Course was not found.");
        return OperationResult.Ok();
    });

    public Task<OperationResult> AddFocusSessionAsync(DateTime startedAtUtc, int minutes, Guid? taskId) =>
        ApplyAsync(data =>
        {
            if (minutes is < 1 or > 480) return OperationResult.Fail("Focus duration must be 1–480 minutes.");
            if (taskId.HasValue && !data.Tasks.Any(t => t.Id == taskId))
                return OperationResult.Fail("The selected task no longer exists.");
            data.FocusSessions.Add(new FocusSession
            {
                StartedAtUtc = startedAtUtc, DurationMinutes = minutes, TaskId = taskId
            });
            return OperationResult.Ok();
        });

    private async Task<OperationResult> ApplyAsync(Func<WorkspaceData, OperationResult> change)
    {
        await InitializeAsync();
        if (!_initialized) return OperationResult.Fail(LoadError ?? "Workspace unavailable.");
        await _writeGate.WaitAsync();
        try
        {
            var copy = _data.Copy();
            var outcome = change(copy);
            if (!outcome.Success) return outcome;
            await _repository.SaveAsync(copy);
            _data = copy;
            Changed?.Invoke();
            return OperationResult.Ok();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Text.Json.JsonException)
        {
            return OperationResult.Fail("Could not save your changes: " + ex.Message);
        }
        finally { _writeGate.Release(); }
    }
}
