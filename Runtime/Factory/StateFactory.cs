using sozooo.GameStateMachine.StateInfrastructure;

#if ZENJECT
using Zenject;
#elif VCONTAINER
using System;
using System.Linq;
using VContainer;
#endif

namespace sozooo.GameStateMachine.Factory
{
    public class StateFactory : IStateFactory
    {
#if ZENJECT
        private readonly IInstantiator _resolver;
        
        public StateFactory(IInstantiator resolver) => 
            _resolver = resolver;
#elif VCONTAINER
        private readonly IObjectResolver _resolver;
        
        public StateFactory(IObjectResolver resolver) => 
            _resolver = resolver;
#endif
        
        public T GetState<T>() where T : class, IExitableState
        {
#if ZENJECT
            return _resolver.Instantiate<T>();
#elif VCONTAINER
            return GetState<T>(null);
#endif
        }

        public T GetState<T>(object[] args) where T : class, IExitableState
        {
#if ZENJECT
            return _resolver.Instantiate<T>(args);
#elif VCONTAINER
            var type = typeof(T);
            var ctor = type.GetConstructors().Single();
            var argMap = args?.ToDictionary(a => a.GetType());
    
            var resolved = ctor.GetParameters().Select(p =>
                argMap != null && argMap.TryGetValue(p.ParameterType, out var val) ? val : _resolver.Resolve(p.ParameterType)
            ).ToArray();

            return (T)Activator.CreateInstance(type, resolved);
#endif
        }
    }
}