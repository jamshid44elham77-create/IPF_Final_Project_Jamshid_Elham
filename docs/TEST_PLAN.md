# Test and quality-assurance checklist

**Execution status:** Not yet verified in the authoring environment (no .NET SDK was installed). Check each box only after you personally run the test. Do not invent evidence or use the checklist as a claim that tests passed.

## A. Automated tests

```bash
dotnet run --project StudyFlow.Tests
```

The source contains 20 test cases (see `StudyFlow.Tests/Program.cs`) covering validation, edge cases, task status changes, priority recommendations, empty dashboards, safe CSV handling, iCalendar export, local persistence, CRUD integration, course deletion constraints, and saved focus sessions. Record your **actual** total from the console.

- [ ] `dotnet build StudyFlow.Web -c Release` succeeds
- [ ] `dotnet build StudyFlow.Tests -c Release` succeeds
- [ ] All automated tests pass (record actual number: ____/____)
- [ ] No unhandled exceptions in terminal during normal use

## B. Manual functional tests

- [ ] First launch creates demo data
- [ ] Dashboard stats match the task list
- [ ] Create a task with no assigned course
- [ ] Create a task with a course, priority and deadline
- [ ] Creating a title of 1 character shows validation
- [ ] Creating an estimate of 999 minutes shows validation
- [ ] Edit title, description, priority, status, and deadline
- [ ] Complete then reopen a task; progress changes
- [ ] Cancel deletion; task remains
- [ ] Confirm deletion; task disappears
- [ ] Search matches title and description (case-insensitive)
- [ ] Filter by course and status; sort by deadline and priority
- [ ] Export CSV and inspect its content
- [ ] Add course; duplicate course is rejected
- [ ] Attempt to delete a course that has tasks; blocked with message
- [ ] Switch calendar months and click deadline days
- [ ] Export `.ics` and import into a calendar app that supports VTODO
- [ ] Start, pause, reset and save a focus session; stats update
- [ ] Restart application; previously saved changes still appear

## C. Cross-platform and responsive checks

Record the OS, browser and device used in your report. A successful compile on one OS is **not proof** of verification on all operating systems.

- [ ] Chrome on Windows
- [ ] Edge or Firefox on Windows
- [ ] A browser on macOS or Linux (if accessible)
- [ ] Desktop layout at 1440px width
- [ ] Tablet layout at 768px width
- [ ] Mobile layout at 390px width
- [ ] Mobile bottom navigation usable with touch
- [ ] Keyboard-only navigation works for forms and buttons
- [ ] Headings, form labels and validation messages are readable
- [ ] Check browser devtools for console errors and network failures

## D. Suggested screenshots for final submission

1. Application dashboard including all four summary cards.
2. New Assignment form, showing the fields.
3. Successful new assignment in task list.
4. Edit form with the same task populated.
5. Search and priority/status filters in action.
6. Calendar showing multiple deadline markers.
7. Courses page with progress bars.
8. Course deletion blocked while assignments remain.
9. Focus timer running.
10. Focus statistics after a saved session.
11. Terminal output of `dotnet build`.
12. Terminal output of `dotnet run --project StudyFlow.Tests`.
13. Browser responsiveness at mobile width.
14. CSV / ICS export evidence.

**Only capture screenshots of the running application and real command output.** Do not claim unrun tests have passed.
