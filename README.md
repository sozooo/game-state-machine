# Game State Machine

[![Readme_RU](https://img.shields.io/badge/GSM-RU-8A2BE2?logo=github)](https://github.com/sozooo/game-state-machine/blob/main/README_RU.md)
![](https://img.shields.io/badge/unity-2022.3+-000.svg)

Dependencies:
* [VContainer](https://github.com/hadashiA/VContainer)
* [C-Sharp-Promise](https://github.com/Real-Serious-Games/C-Sharp-Promise) (already implemented)

## Table of Contents

- [Installation](#installation)
  - [Install via git URL](#install-via-git-url)
  - [Install via manifest](#install-via-manifest)
- [Getting Started](#getting-started)
- [Simple States](#simple-states)
- [Changing States](#changing-states)

Installation
---

Be sure VContainer is installed and VContainer assembly definition is exist

## Install via git URL
Requires a version of unity that supports path query parameter for git packages (Unity >= 2019.3.4f1, Unity >= 2020.1a21). You can add `https://github.com/sozooo/game-state-machine.git` to Package Manager

## Install via manifest
Add `"com.sozooo.game-state-machine": "https://github.com/sozooo/game-state-machine.git"` to `Packages/manifest.json`

Getting Started
---

To get started `GameStateMachine` and `StateFactory` classes must be registered into the container as implemented interfaces (IGameStateMachine)

```csharp
private void RegisterGameStateMachine(IContainerBuilder builder)
{
    builder.Register<GameStateMachine>(Lifetime.Singleton).AsImplementedInterfaces();
    builder.Register<StateFactory>(Lifetime.Singleton).AsImplementedInterfaces();
}
```

And so do custom game states as self, example:

```csharp
private void RegisterGameStates(IContainerBuilder builder)
{
    builder.Register<BootstrapState>(Lifetime.Singleton).AsSelf();
    builder.Register<LoadGameSavesState>(Lifetime.Singleton).AsSelf();
    builder.Register<LoadHomeScreenState>(Lifetime.Singleton).AsSelf();
    builder.Register<HomeScreenState>(Lifetime.Singleton).AsSelf();
    
    builder.Register<LoadBattleState>(Lifetime.Singleton).AsSelf();
    builder.Register<BattleEnterState>(Lifetime.Singleton).AsSelf();
    builder.Register<BattleLoopState>(Lifetime.Singleton).AsSelf();
    
    builder.Register<GameOverState>(Lifetime.Singleton).AsSelf();
}
```

Simple States
---

SimpleState is for states that don't need no payload or end of frame logic:

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

SimplePayloadState is for states that need payload (for example, states that load scenes or modes)

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

EndOfFrameExitState is for states that need to process their exit in the end of a frame

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
See [Examples](https://github.com/sozooo/game-state-machine/tree/main/Examples~) of common states and their connections

Changing States
---

To change state `IGameStateMachine` must be injected into the class. `Enter` method can be callled from state machine instance.

```csharp
_stateMachine.Enter<ENTER_STATE>();
```

`Enter` method is generic; `ENTER_STATES` is needed state
