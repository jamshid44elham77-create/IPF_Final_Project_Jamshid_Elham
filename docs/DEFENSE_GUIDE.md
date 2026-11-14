# Final project defense guide (15–20 minutes)

## Suggested live presentation timing

| Time | What to say or show |
|---|---|
| 0:00–1:30 | Introduce yourself; title "StudyFlow — Student Task Manager"; explain the student problem. |
| 1:30–3:00 | Compare paper planners, spreadsheet tracking and task apps; explain local single-user choice. |
| 3:00–5:00 | Show system architecture: pages → state → C# rules → JSON repository. |
| 5:00–9:00 | Live demo: dashboard; add task, edit task, complete task, filter, search, delete. |
| 9:00–11:00 | Show course tracking, monthly calendar, CSV and iCalendar export. |
| 11:00–12:30 | Show focus timer, logged sessions and smart recommendations. |
| 12:30–14:30 | Explain validation, error handling, responsive design, and secure CSV export. |
| 14:30–16:00 | Run automated tests; describe manual tests actually completed. |
| 16:00–18:00 | Challenges, limitations, future improvements, conclusion; answer questions. |

## Short introduction to practice

"My project is StudyFlow, a Student Task Manager built with C#, .NET 10, and Blazor Interactive Server. Students often struggle to track deadlines across different courses. My application gathers assignments in one place, lets students set priorities and deadlines, recommends what to work on next, and records focus sessions. The data is stored locally using JSON, and the application separates business logic, UI, and persistence through a repository interface."

## Likely instructor questions with sample answers

**1. Why did you choose Blazor?**  Blazor lets me create interactive web pages using C# and Razor components, so I can reuse the same language for UI logic and domain rules.

**2. What is a component?**  A reusable unit of UI and behavior. My `TaskCard` accepts parameters for a task and course and raises `EventCallback` events to edit, delete, or complete a task.

**3. What is `EventCallback`?**  A Blazor mechanism for a child component to notify its parent when an event occurs while keeping the component reusable.

**4. What is dependency injection?**  Instead of creating a repository inside every class, I register `IWorkspaceRepository` in `Program.cs` and receive it in `WorkspaceService` through its constructor.

**5. What is the state container?**  `WorkspaceService` holds the current workspace in memory and raises a `Changed` event when the saved state changes. Pages refresh to show updated data.

**6. Explain SOLID.**  Five design principles. In this project, classes have distinct responsibilities, and the repository interface separates application behavior from how data is stored.

**7. Why use JSON instead of SQL?**  For a local one-person prototype, JSON is simple and avoids database setup. For multiple users or larger datasets I would replace it with a database repository.

**8. How is the data stored?**  After a successful operation, `JsonWorkspaceRepository` writes to a temporary file then replaces the main file. This reduces the risk of a partially written JSON file.

**9. What is a record in C#?**  A type suited to modeling data; my task uses `record` and `with` expressions to create updated copies instead of changing stored task objects directly.

**10. What is validation?**  Checking input before accepting it. The form uses DataAnnotations, and core business rules verify title length, course existence, estimated time and enum values.

**11. How do your smart recommendations work?**  A transparent score prioritizes overdue or soon-due assignments, urgent items, and tasks already in progress. Completed tasks are not suggested.

**12. What is responsive design?**  CSS media queries and flexible grid layouts adapt the navigation and cards for desktop, tablet and mobile screens.

**13. What happens with an empty task list?**  The dashboard shows zero progress safely; task lists display an empty-state message rather than crashing or dividing by zero.

**14. What is `async` / `await`?**  C# syntax for asynchronous operations. I use it to write files without blocking the server thread while the write completes.

**15. What do the CSV and ICS functions do?**  CSV exports assignments into spreadsheet-friendly format, with formula-like values neutralized. ICS creates standard iCalendar to-do items for tasks with deadlines.

**16. Does the project use SignalR?**  Blazor Interactive Server uses a SignalR connection for interactive UI. I did not build a separate SignalR messaging feature.

**17. Is this production ready?**  It is a local single-user educational prototype. It would need authentication, database storage and broader testing before public deployment.

**18. How do you test it?**  The included console test runner checks validation, scoring, exports, file persistence, task CRUD and course safety. I also use a manual browser checklist and record actual test results.


