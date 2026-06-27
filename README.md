# UnityFoundation

Universal Unity 6 game architecture template based on a **ServiceLocator** pattern — no DI framework, no magic, just clean C#.

- **ServiceLocator pattern** — no DI framework. Easier to remove than to add; zero reflection overhead; every dependency is an explicit `ServiceLocator.Get<T>()` call you can trace in one step.
- **8 production-ready services** with clean interfaces and stub implementations — drop in real backends without touching call-sites.
- **Built-in LogSystem** with per-category filtering, crash reporting, and full release-build stripping via the `ENABLE_LOG` scripting define.

## Architecture

```
┌─────────────────────────────────────────────────────┐
│                   GameBootstrap                     │
│          (MonoBehaviour, first scene)               │
└────────────────────┬────────────────────────────────┘
                     │ registers & initializes
                     ▼
┌─────────────────────────────────────────────────────┐
│                 ServiceLocator                      │
│            static Dictionary<Type, IService>        │
└──┬──────┬──────┬──────┬──────┬──────┬──────┬───────┘
   │      │      │      │      │      │      │
   ▼      ▼      ▼      ▼      ▼      ▼      ▼
 Log  Crash Event Data  GSM  Network  UI  Loading
```

### Bootstrap order

```
CrashReporter → LogService → EventBus → DataService
→ GameStateManager → NetworkService → UIService → LoadingService
```

## Services

| Service | Interface | Responsibility |
|---|---|---|
| `LogService` | `ILogService` | Structured logging; categories; stripped in release via `ENABLE_LOG` |
| `CrashReporter` | `ICrashReporter` | Catches unhandled exceptions; queues reports before init |
| `EventBus` | `IEventBus` | Typed pub/sub with priority ordering and `IDisposable` tokens |
| `JsonDataService` | `IDataService` | Async JSON save/load to `persistentDataPath/saves/`; in-memory cache |
| `GameStateManager` | `IGameStateManager` | State machine: Loading → MainMenu → Gameplay → Paused / GameOver |
| `NetworkService` | `INetworkService` | Stub — async connectivity check; `IsConnected` flag |
| `UIService` | `IUIService` | Screen stack per layer (Base / Overlay / HUD / System); 4 canvases |
| `LoadingService` | `ILoadingService` | Show/hide `LoadingScreen` via UIService during async startup |

### UIService layers

| Layer | Screens | Behaviour |
|---|---|---|
| `Base` | MainMenuScreen | One active at a time; Replace clears previous |
| `Overlay` | PauseScreen, GameOverScreen | Stacks on top; Pop removes top |
| `HUD` | HUDScreen | Always visible; immune to HideAll |
| `System` | LoadingScreen | Above everything; immune to HideAll |

## Architecture Decisions

**ServiceLocator over Zenject / Extenject**
DI containers introduce attribute-driven wiring, reflection at startup, and a framework you can't delete. ServiceLocator is 40 lines of C# — readable, debuggable, and replaceable. The trade-off is that dependencies aren't enforced by the constructor, but for a single-executable game with a known boot order that's an acceptable cost.

**`ILogService` + `ICrashReporter` as two separate services**
Logging and crash reporting have different lifecycles. `CrashReporter` must be first in the bootstrap order so it can catch exceptions thrown during `LogService` construction. If they were merged, a log-init failure would be silently swallowed. Separation also lets you swap crash backends (Datadog, Firebase) without touching the log pipeline.

**`IAsyncService` as opt-in, not the base interface**
Only services that genuinely need async initialisation (data load, network handshake) implement `IAsyncService`. Making it universal would force every service to return a `Task`, adding noise and risk of accidental `await` omissions. The bootstrap sequences Critical, Important, and Optional services independently, so sync services pay zero async cost.

**`CrashReporter` pending queue pattern**
Exceptions can be thrown before `LogService` finishes initialising — e.g. in a static constructor or a `Resources.Load` call during service construction. The pending queue absorbs those reports and replays them on `Initialize()`, so no crash is silently lost even in the earliest frames.

**`ScreenLayer` enum with 4 separate Canvas objects**
A single Canvas with `sibling index` tricks breaks the moment any screen dynamically creates children. Four Canvases with fixed `sortingOrder` (0 / 10 / 20 / 30) give guaranteed z-order with no code to maintain. `HideAll` can safely skip HUD and System layers without inspecting individual screens. Each Canvas is `DontDestroyOnLoad` so UI survives scene transitions.

**`EventBus` with priority ordering and `IDisposable` tokens**
Priority ordering lets audio react before UI, and UI before analytics — without coupling those systems. `IDisposable` tokens replace manual `Unsubscribe` calls: store the token in a field, call `Dispose()` in `OnDestroy`, and the subscription is gone. The snapshot-before-iterate pattern (`list.ToArray()`) allows handlers to unsubscribe themselves without invalidating the iterator.

## How to use

1. Clone the repo
2. Open in **Unity 6** (6000.5.x or later)
3. Open `Assets/_Project/Scenes/Bootstrap.unity` and press **Play**

The Bootstrap scene initialises all services, transitions to `GameState.MainMenu`, and loads the MainMenu scene.

## How to add a new service

Use the built-in Claude Code skill:

```
/new-service
```

This walks through creating the interface, implementation, LogCategory, and GameBootstrap registration step by step.

Manual checklist is also available in `.claude/skills/new-service.md`.

## Project structure

```
Assets/_Project/
  Scripts/
    Core/          IService, IAsyncService, ServiceLocator, InitPriority
    Services/
      Log/         LogService, LogCategory, LogSettings, LogExtensions
      Crash/       CrashReporter
      Events/      EventBus (priority, IDisposable tokens)
      Data/        JsonDataService
      State/       GameStateManager, GameState, GameStateChangedEvent
      Network/     NetworkService
      UI/          UIService, IScreen, ScreenLayer, stub screens
      Loading/     LoadingService
    Bootstrap/     GameBootstrap, BootstrapUI, MainMenuUI
    Simulation/    HeadlessSimulation (HEADLESS_SIM define)
  Scenes/          Bootstrap.unity, MainMenu.unity
  Resources/       LogSettings asset
  Tests/
    EditMode/      14 NUnit tests for core systems
```

## Scripting defines

| Define | Platforms | Effect |
|---|---|---|
| `ENABLE_LOG` | All | Enables `.Log()` extension calls |
| `HEADLESS_SIM` | All | Compiles `HeadlessSimulation`; enables **Tools → UnityFoundation → Run Headless Simulation** |

## Running headless simulation

Enable the `HEADLESS_SIM` scripting define (already set), then:

- **In Editor (no Play mode):** Tools → UnityFoundation → Run Headless Simulation
- **At runtime:** `[RuntimeInitializeOnLoadMethod]` fires automatically

## Running tests

Window → General → Test Runner → EditMode → Run All
