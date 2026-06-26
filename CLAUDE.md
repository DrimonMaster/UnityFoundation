# UnityFoundation — Architecture Overview

Universal Unity 6 game template using a **ServiceLocator** pattern (no DI framework).

## Project Layout

```
Assets/
  _Project/
    Scripts/
      Core/          # ServiceLocator, interfaces, base types
      Services/
        Log/         # LogService
        Crash/       # CrashReporter
        State/       # GameStateManager
        Events/      # EventBus
        Data/        # DataService
        Network/     # NetworkService
        UI/          # UIService
      Bootstrap/     # GameBootstrap entry point
    Scenes/          # Game scenes
    Settings/        # ScriptableObject configs
```

## Architecture

### ServiceLocator (`Core/`)

Central registry — no DI framework. Services register themselves and are resolved by interface.

```csharp
ServiceLocator.Register<ILogService>(new LogService());
var log = ServiceLocator.Get<ILogService>();
```

All services implement a common `IService` interface with `Initialize()` and `Dispose()`.

### Bootstrap (`Bootstrap/`)

`GameBootstrap` is the single MonoBehaviour entry point (first scene). It:
1. Instantiates and registers all services in dependency order.
2. Calls `Initialize()` on each.
3. Loads the main game scene.

Order: `LogService` → `CrashReporter` → `EventBus` → `DataService` → `NetworkService` → `GameStateManager` → `UIService`.

### Services

| Service | Interface | Responsibility |
|---|---|---|
| `LogService` | `ILogService` | Structured logging; wraps `Debug.Log` in editor, strips in release builds |
| `CrashReporter` | `ICrashReporter` | Catches unhandled exceptions, reports to analytics/backend |
| `GameStateManager` | `IGameStateManager` | Owns the game state machine (Menu → Loading → Gameplay → Paused → GameOver) |
| `EventBus` | `IEventBus` | Decoupled pub/sub; typed events, no string keys |
| `DataService` | `IDataService` | Save/load game data (local JSON + optional cloud sync) |
| `NetworkService` | `INetworkService` | Wraps Unity Netcode or HTTP client; optional, disabled when offline |
| `UIService` | `IUIService` | Manages UI panels/screens via a stack; drives transitions |

### EventBus pattern

```csharp
// Publishing
EventBus.Publish(new PlayerDiedEvent { Score = 42 });

// Subscribing (unsubscribe in OnDestroy)
EventBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);
```

### Conventions

- All service interfaces live in `Core/`.
- ScriptableObject configs go in `Settings/`; services load them via `DataService` or `Resources`.
- No `FindObjectOfType` outside Bootstrap — always resolve through `ServiceLocator`.
- Scenes go in `_Project/Scenes/`; `Assets/Scenes/` holds the default Unity sample scene only.
- Prefix project-specific assets with nothing (no `_`); the `_Project` folder itself is the namespace boundary.

## Key Files (to be created)

- `Core/IService.cs` — base interface
- `Core/ServiceLocator.cs` — static registry
- `Bootstrap/GameBootstrap.cs` — MonoBehaviour entry point
- `Services/Log/LogService.cs`
- `Services/Crash/CrashReporter.cs`
- `Services/State/GameStateManager.cs`
- `Services/Events/EventBus.cs`
- `Services/Data/DataService.cs`
- `Services/Network/NetworkService.cs`
- `Services/UI/UIService.cs`