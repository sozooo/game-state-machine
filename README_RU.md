# Game State Machine

[![Readme](https://img.shields.io/badge/GSM-EN-8A2BE2?logo=github)](https://github.com/sozooo/game-state-machine/blob/main/README.md)
![](https://img.shields.io/badge/unity-2022.3+-000.svg)
[![Releases](https://shields.io/github/v/release/sozooo/game-state-machine.svg)](https://github.com/sozooo/game-state-machine/releases)

Зависимости:
* [VContainer](https://github.com/hadashiA/VContainer)
* [C-Sharp-Promise](https://github.com/Real-Serious-Games/C-Sharp-Promise) (уже установлено в пакет)

## Содержание

- [Установка](#%D0%A3%D1%81%D1%82%D0%B0%D0%BD%D0%BE%D0%B2%D0%BA%D0%B0)
  - [Установка через git URL](#%D0%A3%D1%81%D1%82%D0%B0%D0%BD%D0%BE%D0%B2%D0%BA%D0%B0-%D1%87%D0%B5%D1%80%D0%B5%D0%B7-git-URL)
  - [Установка через manifest](#%D0%A3%D1%81%D1%82%D0%B0%D0%BD%D0%BE%D0%B2%D0%BA%D0%B0-%D1%87%D0%B5%D1%80%D0%B5%D0%B7-manifest)
  - [Установка через Unity Package](#%D0%A3%D1%81%D1%82%D0%B0%D0%BD%D0%BE%D0%B2%D0%BA%D0%B0-%D1%87%D0%B5%D1%80%D0%B5%D0%B7-unity-package)
- [Начало работы](#%D0%9D%D0%B0%D1%87%D0%B0%D0%BB%D0%BE-%D1%80%D0%B0%D0%B1%D0%BE%D1%82%D1%8B)
- [Простые состояния](#%D0%9F%D1%80%D0%BE%D1%81%D1%82%D1%8B%D0%B5-%D1%81%D0%BE%D1%81%D1%82%D0%BE%D1%8F%D0%BD%D0%B8%D1%8F)
- [Смена состояний](#%D0%A1%D0%BC%D0%B5%D0%BD%D0%B0-%D1%81%D0%BE%D1%81%D1%82%D0%BE%D1%8F%D0%BD%D0%B8%D0%B9)

Установка
---

Be sure VContainer is installed and VContainer assembly definition is exist
Убедитесь, что VContainer установлен и assembly VContainer существует в проекте

## Установка через git URL
Необходима версия Unity, которая поддерживает query параметры для пакетов git (Unity >= 2019.3.4f1, Unity >= 2020.1a21). Можно добавить `https://github.com/sozooo/game-state-machine.git` в Package Manager

## Установка через manifest
Добавьте `"com.sozooo.game-state-machine": "https://github.com/sozooo/game-state-machine.git"` в `Packages/manifest.json`

## Установка через Unity Package
Загрузите файл `.unitypackage`последней версии в [Releases](https://github.com/sozooo/game-state-machine/releases) и импортируйте, как обычный пакет Unity в проект

Начало работы
---

Перед началом `GameStateMachine` и `StateFactory` классы должны быть зарегистрированы в контейнер через реализуемые интерфейсы (IGameStateMachine)

```csharp
private void RegisterGameStateMachine(IContainerBuilder builder)
{
    builder.Register<GameStateMachine>(Lifetime.Singleton).AsImplementedInterfaces();
    builder.Register<StateFactory>(Lifetime.Singleton).AsImplementedInterfaces();
}
```

Также и состояния, в качестве самих себя. Пример:

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

Простые состояния
---

SimpleState для состояний без загрузки или обработки последнего фрейма перед выходом.

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

SimplePayloadState для состояний, которые требуют нагрузку (например, состояния которые загружают сцены или режимы).

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

EndOfFrameExitState для состояний, которым нужно обработать конец фрейма перед выходом.

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
Можно посмотреть [Примеры](https://github.com/sozooo/game-state-machine/tree/main/Examples~) распространенных состояний и их связей

Смена состояний
---

Чтобы сменить состояние `IGameStateMachine` должна быть внедрена в класс. Метод `Enter` может быть вызван из экземпляра машины состояний.

```csharp
_stateMachine.Enter<ENTER_STATE>();
```

Метод `Enter` - дженерик; `ENTER_STATES` - необходимой состояние
