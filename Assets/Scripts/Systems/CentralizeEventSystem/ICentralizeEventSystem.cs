namespace Systems.CentralizeEventSystem
{
    public interface ICentralizeEventSystem
    {
        public void Register(string key, IGameEvent gameEvent);
        
        public SimpleEvent Get(string key);
        public bool TryGet(string key, out SimpleEvent gameEvent);
        
        public SingleParamEvent<T> Get<T>(string key);
        public bool TryGet<T>(string key, out SingleParamEvent<T> gameEvent);
        
        public DoubleParamEvent<T1, T2> Get<T1, T2>(string key);
        public bool TryGet<T1, T2>(string key, out DoubleParamEvent<T1, T2> gameEvent);
        
        public ComplexGameEvent<T1, T2, T3> Get<T1, T2, T3>(string key);
        public bool TryGet<T1, T2, T3>(string key, out ComplexGameEvent<T1, T2, T3> gameEvent);

        public void Unregister(string key);
    }
}