# Orc.Scheduling

Orc.Scheduling is a .NET library that allows easy scheduling of (recurring) tasks inside applications.

It provides:

- `ISchedulingService` / `SchedulingService` — Core service for managing scheduled and recurring tasks.
- `ITimeService` / `TimeService` — Abstraction over the system clock, supports simulated time for testing.
- `ScheduledTask` / `ScheduledTaskBase` — Task model with support for one-shot and recurring execution.

---

## Critical Rules (Read First)

These rules are **non-negotiable**. Violating them causes broken builds, crashes, or downstream breakage.

### 1. Never Edit Generated Files

Files matching `*.generated.cs` are auto-generated.

- **NEVER** manually edit these files

### 2. ABI / API Stability

This project maintains stable ABI / API. Breaking changes break downstream apps.

| Allowed | Never |
|---------|-------|
| Add new overloads | Modify existing signatures |
| Add new methods | Remove public APIs |
| Add new classes | Change return types |

Public API is verified automatically by `PublicApiFacts` tests using `PublicApiGenerator`. If you intentionally change the public API, update the approved snapshot:

```
src/Orc.Scheduling.Tests/PublicApiFacts.Orc_Scheduling_HasNoBreakingChanges_Async.verified.txt
```

### 3. Tests Are Mandatory

**Building alone is NOT sufficient.** Run tests before claiming completion (see [Commands](#commands)).

### 4. Branch Protection (COMPLIANCE REQUIRED)

**Direct commits to protected branches are a policy violation.**

| Repository | Protected Branches |
|------------|-------------------|
| Orc.Scheduling | `master` |
| Orc.Scheduling | `develop` |

**Required workflow:**

1. **Create a feature branch FIRST** — Use naming convention: `feature/issue-NNNN-description`
2. **Make all commits on the feature branch** — Never commit directly to protected branches
3. **Submit a Pull Request** — Changes must be reviewed by a human before merging

```bash
# CORRECT — Always create a feature branch first
git checkout -b feature/issue-1234-fix-description

# NEVER DO THIS — Policy violation
git checkout develop && git commit  # FORBIDDEN

# NEVER DO THIS — Policy violation
git checkout master && git commit  # FORBIDDEN
```

---

## Commands

Single source of truth for all commands:

| Task | Command |
|------|---------|
| **Build** | `dotnet cake --target=build` |
| **Test** | `dotnet cake --target=test` |
| **Build and test** | `dotnet cake --target=buildandtest` |

---

## Architecture & Directories

### Project Overview

```
src/Orc.Scheduling            — Main library (scheduling services, models, extensions)
src/Orc.Scheduling.Tests      — NUnit test project
```

### Directory Guide

| Directory / File | Editable? | Notes |
|------------------|-----------|-------|
| `*.generated.cs` | No | Leave as-is |
| `src/Orc.Scheduling/Services/` | Yes | Core service implementations |
| `src/Orc.Scheduling/Models/` | Yes | Task model classes |
| `src/Orc.Scheduling/Services/Interfaces/` | Yes | Public service interfaces |
| `src/Orc.Scheduling/Services/Extensions/` | Yes | Extension methods for services |
| `src/Orc.Scheduling.Tests/` | Yes | Unit and integration tests |
| `deployment/` | No | Deployment / build scripts |

### Key Types

| Type | Role |
|------|------|
| `ISchedulingService` | Manages adding, removing, starting, and stopping scheduled tasks |
| `SchedulingService` | Default implementation; uses a `Timer` to fire tasks at the right time |
| `ITimeService` | Abstracts the clock; supports simulated time for deterministic tests |
| `TimeService` | Default implementation; supports configurable minute duration for time-acceleration |
| `IScheduledTask` | Contract for a task (start time, recurrence, max duration, action) |
| `ScheduledTask` | Concrete task with a `Func<Task>` action delegate |
| `ScheduledTaskBase` | Abstract base that provides default property implementations |
| `RunningTask` | Tracks a currently-executing task with its `CancellationTokenSource` |

---

## Writing Code

### Anti-Patterns (Never Do This)

| Anti-Pattern | Why |
|-------------|-----|
| Modifying method signatures | ABI breaking |
| Manual edits to `*.generated.cs` | Overwritten on regenerate |
| Using default parameters in public APIs | ABI breaking |
| **Skipping failing tests** | **Unacceptable — tests must pass** |
| Blocking inside `InvokeAsync` | Tasks run on the thread pool; blocking stalls the scheduler |

---

## Testing & Debugging

### Running Tests

```bash
dotnet cake --target=test
```

### Tests MUST Pass

> **NON-NEGOTIABLE:** Tests must PASS before claiming completion.
>
> - Do NOT skip failing tests
> - Do NOT claim completion if tests fail
> - Do NOT use `SkipException` to work around failures

### Writing Tests

1. Use NUnit to write tests
2. Create a Facts class for a feature
3. Combine Pascal / Snake case for test methods (e.g. `Feature_Does_Work`)

```csharp
[Test]
public async Task SchedulingService_Executes_Task_At_Scheduled_Time_Async()
{
    var timeService = new TimeService(TimeSpan.FromMilliseconds(50));
    var schedulingService = new TestSchedulingService(NullLogger<SchedulingService>.Instance, timeService);

    var invoked = false;
    var task = new ScheduledTask
    {
        Name = "Test task",
        Start = timeService.CurrentDateTime,
        Action = () =>
        {
            invoked = true;
            return Task.CompletedTask;
        }
    };

    schedulingService.AddScheduledTask(task);

    await Task.Delay(TimeSpan.FromMilliseconds(500));

    Assert.That(invoked, Is.True);
}
```

**Philosophy:** Tests FAIL when wrong, never skip (except missing hardware).

### Simulated Time in Tests

Use `TimeService` with a custom `minuteDuration` to speed up time-based tests:

```csharp
// 1 simulated minute = 50 ms of real time
var timeService = new TimeService(TimeSpan.FromMilliseconds(50));
var schedulingService = new TestSchedulingService(logger, timeService);
```

This makes recurring task tests run in milliseconds instead of minutes.

### Public API Snapshot Tests

`PublicApiFacts` uses `PublicApiGenerator` + `Verify` to detect unintentional breaking changes.  
If you intentionally change the public API, update the snapshot file:

```
src/Orc.Scheduling.Tests/PublicApiFacts.Orc_Scheduling_HasNoBreakingChanges_Async.verified.txt
```

### Debugging Methodology

1. **Establish baseline** — What's the known-good state?
2. **One change at a time** — Verify each change before proceeding
3. **Track changes in a table** — Log what you changed and the result
4. **Platform differences are signals** — If X works and Y fails, the difference IS the answer
5. **Revert if worse** — Don't pile fixes on top of failures

---

## Further Reading

| Topic | Document |
|-------|---------|
| Contributing guidelines | [CONTRIBUTING.md](CONTRIBUTING.md) |
| Project homepage | [WildGums Open Source](http://opensource.wildgums.com) |
