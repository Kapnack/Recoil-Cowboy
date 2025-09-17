using System;

namespace Systems.CentralizeEventSystem
{
    public class ComplexGameEvent<T1, T2, T3> : IGameEvent
    {
        public event Action<T1, T2, T3> Listeners;
        
        public void Invoke(T1 a, T2 b, T3 c) => Listeners?.Invoke(a, b, c);
        public void AddListener(Action<T1, T2, T3> action) => Listeners += action;
        public void RemoveListener(Action<T1, T2, T3> action) => Listeners -= action;
        
        public bool HasInvocations() => Listeners != null && Listeners.GetInvocationList().Length > 0;
        
    }
}