# Mapping the instructor's 20 grading criteria to StudyFlow

This tracks the uploaded final project requirements. Each criterion represents 5% of the course rubric. Features listed are planned/implemented in the prepared source; **actual runtime behavior and quality are subject to student verification**.

| # | Rubric criterion | Relevant implementation / evidence | Gaps or verification needed |
|---:|---|---|---|
| 1 | Code structure and design | Separate Core, Web, and Tests; repository interface; SOLID discussion | Review and compile |
| 2 | Reusable components | `TaskCard`, `TaskEditor`, `StatTile`, `EmptyState`, parameters/events | Test UI interactions |
| 3 | Cross-platform | .NET 10 and browser-based UI; no OS-native APIs | Test on multiple OSes |
| 4 | State management | Scoped `WorkspaceService`, immutable task records, `Changed` notifications | Test navigation and concurrent windows |
| 5 | UI design | Custom responsive CSS, consistent tokens, navigation and cards | Visual audit in browser |
| 6 | UX | Form validation, delete confirmation, empty states, filtering | Keyboard and usability testing |
| 7 | Responsiveness | CSS grid, breakpoints at 1180/930/650px, mobile navigation | Test phones/tablets |
| 8 | Core functionality | Task CRUD, statuses, course organization, calendar, search | Run functional checklist |
| 9 | Advanced features | Deadline-weighted recommendations, focus timer, CSV and ICS | Demo and check exports |
| 10 | Error handling | Validation, duplicate detection, safe course deletion, storage error handling | Test read-only storage, corrupted JSON |
| 11 | Efficiency | In-memory view state, no repeated network lookups or large dependencies | Profile with real data |
| 12 | Scalability | Layered repository abstraction supports future database | Current JSON storage is single-user only |
| 13 | Resource use | Minimal JS, no external font/library calls, local storage | Measure actual CPU/memory |
| 14 | Testing coverage | Dependency-free automated tests plus manual plan | Not xUnit or browser E2E; execute tests |
| 15 | Quality assurance | Detailed test matrix in `TEST_PLAN.md` | Cross-browser/device checks pending |
| 16 | Documentation | README, architecture, test plan, grading matrix, defense notes | Review installation steps |
| 17 | Innovation | Explainable study-priority score and combined focus tracking | Explain original rationale |
| 18 | Creative Blazor/C# | Interactive Server, reusable typed components, CSS ring, event callbacks | Demonstrate live UI updates |
| 19 | Professionalism | Clear UI system, error notices, exports, documented limitations | Verify / polish after running |
| 20 | Presentation | Included slide deck and defense questions | Practice required 15–20-minute presentation |

**Important honesty note:** This mapping does not mean an automatic 100/100. The rubric's highest grade explicitly expects broad testing and production-grade readiness. This local app needs authentication, database concurrency, browser E2E automation, and actual cross-platform testing for a credible claim of fully meeting every top-level standard.
