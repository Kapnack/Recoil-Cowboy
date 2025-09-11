using System;

namespace Systems.CentralizeEventSystem
{
    public class SingleParamEvent<T> : IGameEvent
    {
        public event Action<T> Listeners;
        
        public void Invoke(T a) => Listeners?.Invoke(a);
        public void AddListener(Action<T> action) => Listeners += action;
        public void RemoveListener(Action<T> action) => Listeners -= action;
    }
}