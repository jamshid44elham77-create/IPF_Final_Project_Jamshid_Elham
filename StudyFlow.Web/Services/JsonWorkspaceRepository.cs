using System.Text.Json;
using System.Text.Json.Serialization;
using StudyFlow.Core.Models;
using StudyFlow.Core.Services;

namespace StudyFlow.Web.Services;

public sealed class JsonWorkspaceRepository : IWorkspaceRepository
{
    private readonly string _path;
    private readonly bool _seedIfMissing;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public JsonWorkspaceRepository(string path, bool seedIfMissing = true)
    {
        _path = path;
        _seedIfMissing = seedIfMissing;
    }

    public async Task<WorkspaceData> ReadAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            if (!File.Exists(_path))
            {
                var initial = _seedIfMissing ? SampleData.Create(DateTime.Today) : new WorkspaceData();
                await WriteCoreAsync(initial, cancellationToken);
                return initial;
            }
            await using var stream = File.OpenRead(_path);
            var data = await JsonSerializer.DeserializeAsync<WorkspaceData>(stream, Options, cancellationToken);
            return data ?? throw new InvalidDataException("The saved workspace is empty or invalid.");
        }
        finally { _gate.Release(); }
    }

    public async Task SaveAsync(WorkspaceData data, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try { await WriteCoreAsync(data, cancellationToken); }
        finally { _gate.Release(); }
    }

    private async Task WriteCoreAsync(WorkspaceData data, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        var temp = _path + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            await using (var stream = new FileStream(temp, FileMode.CreateNew, FileAccess.Write,
                FileShare.None, 4096, useAsync: true))
            {
                await JsonSerializer.SerializeAsync(stream, data, Options, cancellationToken);
                await stream.FlushAsync(cancellationToken);
            }
            File.Move(temp, _path, overwrite: true);
        }
        finally { if (File.Exists(temp)) File.Delete(temp); }
    }
}
