# Game State Machine

[![Readme](https://img.shields.io/badge/GSM-EN-8A2BE2?logo=github)](https://github.com/sozooo/game-state-machine/blob/main/README.md)
![](https://img.shields.io/badge/unity-2022.3+-000.svg)
[![Releases](https://shields.io/github/v/release/sozooo/game-state-machine.svg)](https://github.com/sozooo/game-state-machine/releases)

Зависимости:
* DI-контейнер (один из двух представленных):
  * [VContainer](https://github.com/hadashiA/VContainer)
  * [Zenject (Extenject)](https://github.com/undsoft/Extenject) (важно устанавливать его через [UPM или manifest.json](https://github.com/Mathijs-Bakker/Extenject/blob/master/README.md#installation-))
* [C-Sharp-Promise](https://github.com/Real-Serious-Games/C-Sharp-Promise) (уже установлено в пакет)

## Содержание

- [Установка](#%D0%A3%D1%81%D1%82%D0%B0%D0%BD%D0%BE%D0%B2%D0%BA%D0%B0)
- [Начало работы](#%D0%9D%D0%B0%D1%87%D0%B0%D0%BB%D0%BE-%D1%80%D0%B0%D0%B1%D0%BE%D1%82%D1%8B)
- [Простые состояния](#%D0%9F%D1%80%D0%BE%D1%81%D1%82%D1%8B%D0%B5-%D1%81%D0%BE%D1%81%D1%82%D0%BE%D1%8F%D0%BD%D0%B8%D1%8F)
- [Смена состояний](#%D0%A1%D0%BC%D0%B5%D0%BD%D0%B0-%D1%81%D0%BE%D1%81%D1%82%D0%BE%D1%8F%D0%BD%D0%B8%D0%B9)

Установка
---

Убедитесь, что VContainer или Extenject установлены и их assembly существует в проекте. Пакет автоматически определяет установленный **DI-фреймворк** через versionDefines в asmdef.

1.  __Установка через git URL.__

    * Необходима версия Unity, которая поддерживает query параметры для пакетов git (Unity >= 2019.3.4f1, Unity >= 2020.1a21).
      * Window -> Package Manager;
      * Необходимо выбрать `Add package from git URL...`;
      * Используйте `https://github.com/sozooo/game-state-machine.git`.
      <br>
      <img width="422" height="166" alt="image" src="https://github.com/user-attachments/assets/3836a472-1400-47a8-a31d-f162ac41b173" />

1.  __Установка через manifest.json.__

    * Необходимо добавить `"com.sozooo.game-state-machine": "https://github.com/sozooo/game-state-machine.git"` в `Packages/manifest.json`.

1.  __Установка через Unity Package.__

    * Загрузите файл `.unitypackage` последней версии в [Releases](https://github.com/sozooo/game-state-machine/releases) и импортируйте, как обычный Unity-пакет в проект.

Начало работы
---

Перед началом `GameStateMachine` и `StateFactory` классы должны быть зарегистрированы в контейнер через реализуемые интерфейсы, `Singleton`.

`GameStateMachine` зависит от `StateMachine`, но `StateMachine` можно переиспользовать, как обычную машину состояний для любых существ. Поэтому ее нужно зарегистрировать, как `Transient`:

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

_Регистрировать состояния не нужно._ В версии **1.2** и старше создание состояний происходит в `StateFactory` через рефлексию.

Простые состояния
---

`SimpleState` для состояний без загрузки или обработки последнего фрейма перед выходом.

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

`SimplePayloadState` для состояний, которые требуют нагрузку _(например, состояния которые загружают сцены или режимы)_.

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

`EndOfFrameExitState` для состояний, которым необходим Update и остановка в конце кадра перед выходом.

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
Можно посмотреть [Примеры](https://github.com/sozooo/game-state-machine/tree/main/Examples~) распространенных состояний и их связей. Также в папке `1.2` можно посмотреть реализацию патрулирования врага, в котором задействованы дополнительные аргументы для инъекции в состояние.

Смена состояний
---

Чтобы сменить состояние `IStateMachine` должна быть внедрена в класс. Метод `Enter` может быть вызван из экземпляра машины состояний.

```csharp
_stateMachine.Enter<ENTER_STATE>();
```

Метод `Enter` - дженерик; `ENTER_STATES` - необходимой состояние, имплементирующее интерфейс `IState`.

У метода `Enter` есть перегрузка в случае если `ENTER_PAYLOAD_STATE` имплементирует `IPayloadState<TPayload>` или наследует `SimplePayloadState<TPayload>`.

```csharp
_stateMachine.Enter<ENTER_PAYLOAD_STATE>(PAYLOAD)
```

`StateMachine` сделает запрос на вхождение в новое состояние. Если уже было активное состояние `_activeState`, машина вызовет в нем `IExitableState.BeginExit()` и будет создан `Promise`. Когда `Promise` входа из состояния будет решен (вызовется `Resolve`), машина вызовет `IExitableState.EndExit()` на активном состоянии. После этого будет применено необходимое состояние и у него вызовется метод `Enter`.

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
Для управления состоянием игры используется Singleton `IGameStateMachine`. Он вызывает внутренний `IStateMachine` и переключает состояния:
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
