using UnityEngine;

namespace Systems.Pool
{
    public interface IPool<T>
    {
        public void InitializeRandom(int amount, int offset = 0);
        public void InitializeAll(int offset = 0);
        public T Get(int offset = 0);
        public void Return(T data);
        public void Clear();
    }

    public struct PoolData<T>
    {
        public PoolData(GameObject obj)
        {
            Obj = obj;
            Component = obj ? obj.gameObject.GetComponent<T>() : default;
        }

        public GameObject Obj;
        public T Component;
    }
}