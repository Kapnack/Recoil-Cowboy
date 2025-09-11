using System.Collections.Generic;
using UnityEngine;

namespace Systems.CentralizeEventSystem
{
    public class CentralizeEventSystem : MonoBehaviour, ICentralizeEventSystem
    {
        private readonly Dictionary<string, IGameEvent> events = new();

        private void Awake()
        {
            ServiceProvider.SetService<ICentralizeEventSystem>(this);
        }

        public void Register(string key, IGameEvent gameEvent)
        {
            if (!events.TryAdd(key, gameEvent))
                Debug.LogWarning($"{key} Event with key '{key}' already registered.");
        }

        public SimpleEvent Get(string key)
        {
            return events.TryGetValue(key, out var e) ? e as SimpleEvent : null;
        }

        public SingleParamEvent<T> Get<T>(string key)
        {
            return events.TryGetValue(key, out var e) ? e as SingleParamEvent<T> : null;
        }

        public ComplexGameEvent<T1, T2, T3> Get<T1, T2, T3>(string key)
        {
            return events.TryGetValue(key, out var e) ? e as ComplexGameEvent<T1, T2, T3> : null;
        }

        public void Unregister(string key) => events.Remove(key);
    }
}
