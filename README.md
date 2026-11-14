# StudyFlow — Student Task Manager

**Final individual project | C# · .NET 10 · Blazor Interactive Server**

StudyFlow is a responsive local web application for students who need to organize assignments, deadlines, and course work. It includes CRUD task management, priority-based recommendations, course progress, a monthly calendar, a focus timer, study-session statistics, CSV export, and iCalendar (`.ics`) export.

> **Important:** The source code and teaching materials are prepared, but the project could **not be compiled or executed in the authoring environment because the .NET SDK was unavailable**. Run the build and tests on your computer, inspect the actual interface, fix any environment-specific issues.

## 1. Requirements

- .NET 10 SDK: https://dotnet.microsoft.com/download/dotnet/10.0
- Windows, macOS, or Linux (a browser is needed)
- VS Code with C# Dev Kit, or Visual Studio / Rider (optional)
- No database server, cloud service, or paid packages

## 2. Run the application (VS Code terminal)

Open a terminal in the **StudyFlow_Final_Project** folder:

```bash
dotnet --version
dotnet restore StudyFlow.Web/StudyFlow.Web.csproj
dotnet run --project StudyFlow.Web
```

The terminal shows the local address, typically `http://localhost:5xxx` or `https://localhost:7xxx`. Open that exact address in Chrome, Edge, Firefox, or Safari. Press `Ctrl+C` to stop.

If the browser warns about an HTTPS development certificate, follow the official `dotnet dev-certs https --trust` guidance for your OS; use the HTTP address if the terminal provides one.

### Run automated tests

```bash
dotnet run --project StudyFlow.Tests
```

The test harness exits with status `0` only if all cases pass. It covers validation, recommendations, statistics, secure exports, repository round trips, and service-level CRUD integration.

### Production-style compile

```bash
dotnet build StudyFlow.Web/StudyFlow.Web.csproj -c Release
dotnet build StudyFlow.Tests/StudyFlow.Tests.csproj -c Release
```

## 3. First use

On first run, StudyFlow creates demonstration courses and example tasks with deadlines relative to the current date. You can edit/delete them and add your real courses. The app automatically saves every successful change.

**Data file:** `StudyFlow.Web/App_Data/workspace.json` (created when running). To reset the demonstration data, stop the app, back up the file if needed, then delete `workspace.json` and start again.

**Protect your data:** this is an individual local student demo; no authentication is implemented. Do not deploy it on the public Internet or use it for multi-user data without adding login, authorization, database concurrency, and security review.

## 4. Features and how to demo them

| Screen | Student action | Result |
|---|---|---|
| Dashboard | Open `/` | Counts, progress, overdue notices, suggested next tasks, course status |
| My tasks | Click **New assignment** | Create a validated task with optional course, due date, status, priority, estimate |
| My tasks | Click edit / checkbox / delete | Edit, mark complete / reopen, or delete with confirmation |
| My tasks | Type in search; change filters | List responds immediately to keywords, status, and course |
| My tasks | Click **Export CSV** | Saves a spreadsheet-compatible task list |
| Calendar | Select a day; navigate months | View date-based assignments and export `.ics` |
| Courses | Create course | Groups tasks by named subject and shows progress |
| Courses | Delete an occupied course | Safe error prevents orphaning assignments |
| Focus | Choose duration, task, start | Countdown; logs completed sessions or full elapsed minutes when stopped |
| Focus | Open summary | See total and today's saved focus minutes |

## 5. Project structure

```text
StudyFlow_Final_Project/
├── StudyFlow.Core/             # Models, validation, recommendation rules, exports
│   ├── Models/
│   └── Services/
├── StudyFlow.Web/              # ASP.NET Core Blazor Interactive Server application
│   ├── Components/
│   │   ├── Layout/            # Navigation and adaptive shell
│   │   ├── Pages/             # Dashboard, Tasks, Courses, Calendar, Focus, About
│   │   └── Shared/            # TaskCard, TaskEditor, StatTile, EmptyState
│   ├── Services/               # Scoped state container, interface, JSON repository
│   └── wwwroot/                # Styles, icons, and browser download helper
├── StudyFlow.Tests/            # No-external-package automated test runner
├── docs/                       # Architecture, QA checklist, defense and evaluation
├── Presentation/               # Presentation deck
└── README.md
```

## 6. Architecture and C# concepts

- **Presentation:** `.razor` components and responsive CSS, with `EventCallback` for reusable UI events.
- **State management:** the scoped `WorkspaceService` contains a current snapshot and broadcasts a `Changed` event to UI pages.
- **Business layer:** pure `TaskRules`, `TaskInsights`, and `ExportService` methods in a reusable .NET class library.
- **Persistence layer:** `IWorkspaceRepository` abstraction; `JsonWorkspaceRepository` serializes the snapshot. Saving creates a temporary file and atomically replaces the old file.
- **Dependency injection:** startup registers repository and workspace service in `Program.cs`.
- **Validation:** DataAnnotations on forms plus domain validation before persistence.
- **Advanced feature:** deterministic deadline-aware recommendation scores, a cancellable focus timer, safe CSV export, ICS calendar export.

## 7. Known constraints

This application is a **single-user demo**, not a production internet-facing service. JSON file storage is intentionally simple but is not a scalable multi-user database. The focus countdown requires an active browser/Blazor Server connection. It does not offer accounts, cloud sync, notifications, or SignalR-based multi-user real-time updates. Blazor Interactive Server itself uses a SignalR connection for UI interaction, but the project does not implement a custom real-time collaboration channel.

## 8. Submission and defense

Bring the application to your defense and demonstrate adding, editing, filtering, completing, and deleting a task; the smart recommendation update; course safety; calendar export; and a focus session. Use the included PowerPoint and the defense notes in `docs/DEFENSE_GUIDE.md` as a foundation. Run the tests yourself and record the actual output. The course instructions make an oral defense compulsory.

See `docs/ARCHITECTURE.md`, `docs/TEST_PLAN.md`, `docs/GRADING_MATRIX.md`, and `docs/DEFENSE_GUIDE.md` for complete details.
