using System;
using System.Collections.Generic;

namespace Systems
{
    public static class ServiceProvider
    {
        private static readonly Dictionary<Type, object> Services = new();

        public static void SetService<T>(T service, bool overrideIfFound = false)
        {
            if (!Services.TryAdd(typeof(T), service) && overrideIfFound)
                Services[typeof(T)] = service;
        }

        public static bool TryGetService<T>(out T service) where T : class
        {
            if (Services.TryGetValue(typeof(T), out var serviceObject) && serviceObject is T tService)
            {
                service = tService;
                return true;
            }

            service = null;
            return false;
        }

        public static void RemoveService<T>() => Services.Remove(typeof(T));
    }
}