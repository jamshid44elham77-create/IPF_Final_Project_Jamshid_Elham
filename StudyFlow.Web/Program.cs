using StudyFlow.Web.Components;
using StudyFlow.Web.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// Local, single-user JSON persistence. No third-party NuGet dependency is needed.
builder.Services.AddSingleton<IWorkspaceRepository>(_ =>
    new JsonWorkspaceRepository(Path.Combine(builder.Environment.ContentRootPath, "App_Data", "workspace.json")));
builder.Services.AddScoped<WorkspaceService>();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapStaticAssets();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();
