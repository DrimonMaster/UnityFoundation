# UnityFoundation

Universal Unity 6 game architecture template based on a **ServiceLocator** pattern — no DI framework, no magic, just clean C#.

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
