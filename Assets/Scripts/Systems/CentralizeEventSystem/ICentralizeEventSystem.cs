namespace Systems.CentralizeEventSystem
{
    public interface ICentralizeEventSystem
    {
        public void Register(string key, IGameEvent gameEvent);
        public SimpleEvent Get(string key);
        public SingleParamEvent<T> Get<T>(string key);
        public ComplexGameEvent<T1, T2, T3> Get<T1, T2, T3>(string key);

        public void Unregister(string key);
    }
}