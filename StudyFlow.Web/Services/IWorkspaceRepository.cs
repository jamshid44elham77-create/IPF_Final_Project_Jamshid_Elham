using StudyFlow.Core.Models;

namespace StudyFlow.Web.Services;

public interface IWorkspaceRepository
{
    Task<WorkspaceData> ReadAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(WorkspaceData data, CancellationToken cancellationToken = default);
}
