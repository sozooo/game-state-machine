# Game State Machine

[![Readme_RU](https://img.shields.io/badge/GSM-RU-8A2BE2?logo=github)](https://github.com/sozooo/game-state-machine/blob/main/README_RU.md)
![](https://img.shields.io/badge/unity-2022.3+-000.svg)
[![Releases](https://shields.io/github/v/release/sozooo/game-state-machine.svg)](https://github.com/sozooo/game-state-machine/releases)

Dependencies:
* DI container (choose one of the following):
  * [VContainer](https://github.com/hadashiA/VContainer)
  * [Zenject (Extenject)](https://github.com/undsoft/Extenject) (must be installed [via UPM or manifest.json](https://github.com/Mathijs-Bakker/Extenject/blob/master/README.md#installation-))
* [C-Sharp-Promise](https://github.com/Real-Serious-Games/C-Sharp-Promise) (already included in the package)

## Table of Contents

- [Installation](#installation)
- [Getting Started](#getting-started)
- [Simple States](#simple-states)
- [Changing States](#changing-states)

Installation
---

Make sure VContainer or Extenject is installed and its assembly exists in the project. The package automatically detects the installed **DI framework** via `versionDefines` in the asmdef.

1.  __Install via git URL.__

    * Requires a version of Unity that supports query parameters for git packages (Unity >= 2019.3.4f1, Unity >= 2020.1a21).
      * Window -> Package Manager;
      * Select `Add package from git URL...`;
      * Use `https://github.com/sozooo/game-state-machine.git`.
      <br>
      <img width="422" height="166" alt="image" src="https://github.com/user-attachments/assets/3836a472-1400-47a8-a31d-f162ac41b173" />

1.  __Install via manifest.json.__

    * Add `"com.sozooo.game-state-machine": "https://github.com/sozooo/game-state-machine.git"` to `Packages/manifest.json`.

1.  __Install via Unity Package.__

    * Download the `.unitypackage` file of the latest version from [Releases](https://github.com/sozooo/game-state-machine/releases) and import it as a regular Unity package into the project.

Getting Started
---

Before getting started, `GameStateMachine` and `StateFactory` classes must be registered in the container via their implemented interfaces as `Singleton`.

`GameStateMachine` depends on `StateMachine`, but `StateMachine` can be reused as a regular state machine for any entities. Therefore, it should be registered as `Transient`:

**VContainer**
```csharp
private void RegisterGameStateMachine(IContainerBuilder builder)
{
    builder.Register<StateFactory>(Lifetime.Singleton).AsImplementedInterfaces();
    builder.Register<StateMachine>(Lifetime.Transient).AsImplementedInterfaces();
    builder.Register<GameStateMachine>(Lifetime.Singleton).AsImplementedInterfaces();
}
```
**Extenject**
```csharp
public override void InstallBindings()
{
    Container.BindInterfacesAndSelfTo<StateFactory>().AsSingle();
    Container.BindInterfacesAndSelfTo<StateMachine>().AsTransient();
    Container.BindInterfacesAndSelfTo<GameStateMachine>().AsSingle();
}
```

_No need to register states._ In version **1.2** and later, states are created in `StateFactory` via reflection.

Simple States
---

`SimpleState` is for states that don't need loading or end-of-frame processing before exiting.

```csharp
public class SimpleState : IState
{
    public virtual void Enter()
    {
    }

    protected virtual void Exit()
    {
    }

    IPromise IExitableState.BeginExit()
    {
      Exit();
      return Promise.Resolved();
    }

    void IExitableState.EndExit()
    {
    }
}
```

`SimplePayloadState` is for states that require a payload _(e.g., states that load scenes or modes)_.

```csharp
public class SimplePayloadState<TPayload> : IPayloadState<TPayload>
{
    public virtual void Enter(TPayload payload)
    {
    }

    protected virtual void Exit()
    {
    }

    IPromise IExitableState.BeginExit()
    {
      Exit();
      return Promise.Resolved();
    }

    void IExitableState.EndExit()
    {
    }
}
```

`EndOfFrameExitState` is for states that need `Update` and a frame-end stop before exiting.

```csharp
public class EndOfFrameExitState : IState, IUpdateable
{ 
    private Promise _exitPromise;

    private bool ExitWasRequested =>
        _exitPromise != null;

    public virtual void Enter()
    {
    }

    IPromise IExitableState.BeginExit()
    {
        _exitPromise = new Promise();
        return _exitPromise;
    }

    void IExitableState.EndExit()
    {
        ExitOnEndOfFrame();
        ClearExitPromise();
    }

    void IUpdateable.Update()
    {
        if (!ExitWasRequested)
            OnUpdate();
      
        if (ExitWasRequested) 
            ResolveExitPromise();
    }

    protected virtual void ExitOnEndOfFrame()
    {
    }

    protected virtual void OnUpdate()
    {
    }

    private void ClearExitPromise() =>
        _exitPromise = null;

    private void ResolveExitPromise() =>
        _exitPromise?.Resolve();
}
```

See [Examples](https://github.com/sozooo/game-state-machine/tree/main/Examples~) of common states and their connections. Also, in the `1.2` folder you can find an enemy patrolling implementation that uses additional constructor arguments for state injection.

Changing States
---

To change a state, `IStateMachine` must be injected into the class. The `Enter` method can be called from the state machine instance.

```csharp
_stateMachine.Enter<ENTER_STATE>();
```

The `Enter` method is generic; `ENTER_STATE` is the desired state implementing the `IState` interface.

The `Enter` method has an overload if `ENTER_PAYLOAD_STATE` implements `IPayloadState<TPayload>` or inherits from `SimplePayloadState<TPayload>`:

```csharp
_stateMachine.Enter<ENTER_PAYLOAD_STATE>(PAYLOAD)
```

`StateMachine` will request entering a new state. If there is already an active `_activeState`, the machine calls `IExitableState.BeginExit()` on it and a `Promise` is created. When the exit `Promise` is resolved, the machine calls `IExitableState.EndExit()` on the active state. After that, the new state is applied and its `Enter` method is called.

```csharp
public void Enter<TState>() where TState : class, IState =>
    RequestEnter<TState>()
        .Done();

private IPromise<TState> RequestEnter<TState>() where TState : class, IState =>
    RequestChangeState<TState>()
        .Then(EnterState);

private IPromise<TState> RequestChangeState<TState>() where TState : class, IExitableState
{
    if (_activeState != null)
    {
        return _activeState
            .BeginExit()
            .Then(_activeState.EndExit)
            .Then(ChangeState<TState>);
    }
      
    return ChangeState<TState>();
}

private IPromise<TState> ChangeState<TState>() where TState : class, IExitableState
{
    TState state = _stateFactory.GetState<TState>();

    return Promise<TState>.Resolved(state);
}
```

A singleton `IGameStateMachine` is used to manage the game state. It wraps the internal `IStateMachine` and switches states:
```csharp
public class GameStateMachine : IGameStateMachine, ITickable
{
    private readonly IStateMachine _stateMachine;
        
    public GameStateMachine(IStateMachine stateMachine) => 
        _stateMachine = stateMachine;

    public void Tick() => 
        _stateMachine.Tick();

    public void Enter<TState>() where TState : class, IState =>
        _stateMachine.Enter<TState>();

    public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload> =>
        _stateMachine.Enter<TState, TPayload>(payload);
}
```
