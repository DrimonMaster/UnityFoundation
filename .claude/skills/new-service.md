# Skill: Create a New Service

Follow these steps in order when adding a new service to UnityFoundation.

## 1. Interface — `IServiceName.cs`

Create in `Assets/_Project/Scripts/Services/<Category>/IServiceName.cs`:

```csharp
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public interface IServiceName : IService   // or IAsyncService if StartAsync needed
    {
        // public API
    }
}
```

## 2. Implementation — `ServiceName.cs`

Create in the same folder:

```csharp
using UnityFoundation.Core;

namespace UnityFoundation.Services
{
    public class ServiceName : IServiceName
    {
        public InitPriority Priority => InitPriority.Critical; // or Important / Optional
        public bool IsReady { get; private set; }

        public void Initialize()
        {
            IsReady = true;
            "[Lifecycle] ServiceName initialized".Log(LogCategory.Lifecycle);
        }

        public void Dispose()
        {
            "[Lifecycle] ServiceName disposed".Log(LogCategory.Lifecycle);
            IsReady = false;
        }

        // If IAsyncService:
        // public async Task StartAsync()
        // {
        //     await Task.CompletedTask;
        //     IsReady = true;
        //     "[Lifecycle] ServiceName ready".Log(LogCategory.Lifecycle);
        // }
    }
}
```

## 3. LogCategory (if new category needed)

Add to `Assets/_Project/Scripts/Services/Log/LogCategory.cs`:

```csharp
public enum LogCategory
{
    // ... existing ...
    ServiceName   // add here
}
```

## 4. Register in GameBootstrap

Open `Assets/_Project/Scripts/Bootstrap/GameBootstrap.cs` → `RegisterServices()`.

Add in the correct dependency order (LogService and EventBus are always first):

```csharp
Add<IServiceName>(new ServiceName());
```

Bootstrap order rule:
- `Critical` services that others depend on → first
- `Important` async services (network, data) → middle  
- `Optional` background services → last

## 5. Update CLAUDE.md services table

Add a row to the Services table in `CLAUDE.md`:

```
| `ServiceName` | `IServiceName` | One-line description |
```

## Checklist

- [ ] `IServiceName.cs` exists next to `ServiceName.cs`
- [ ] `Priority` and `IsReady` implemented
- [ ] `Initialize()` sets `IsReady = true` and logs with `LogCategory.Lifecycle`
- [ ] `Dispose()` sets `IsReady = false` and logs with `LogCategory.Lifecycle`
- [ ] Registered in `GameBootstrap.RegisterServices()` in correct order
- [ ] CLAUDE.md services table updated
