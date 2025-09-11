using System;

namespace Systems.CentralizeEventSystem
{
    public class SimpleEvent : IGameEvent
    {
        public event Action Listeners;

        public void Invoke() => Listeners?.Invoke();
        public void AddListener(Action action) => Listeners += action;
        public void RemoveListener(Action action) => Listeners -= action;
    }
}