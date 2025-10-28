using System;

namespace Systems.CentralizeEventSystem
{
    public class DoubleParamEvent<T1, T2> : IGameEvent
    {
        public event Action<T1, T2> Listeners;
        
        public void Invoke(T1 a, T2 b) => Listeners?.Invoke(a, b);
        public void AddListener(Action<T1, T2> action) => Listeners += action;
        public void RemoveListener(Action<T1, T2> action) => Listeners -= action;
        
        public bool HasInvocations() => Listeners != null && Listeners.GetInvocationList().Length > 0;
    }
}