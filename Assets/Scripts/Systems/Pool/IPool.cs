using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Systems.Pool
{
    public interface IPool<T>
    {
        public void InitializeRandom(int amount, int offset = 0);
        public void InitializeAll(int repeat = 0, int offset = 0);
        public Task<T> Get(int offset = 0);
        public void Return(T data);
        public void Clear();
    }

    public class PoolData<T>
    {
        public PoolData(GameObject obj)
        {
            Obj = obj;
            Component = obj ? obj.gameObject.GetComponent<T>() : default;
        }

        public readonly GameObject Obj;
        public readonly T Component;
        
        public static implicit operator bool(PoolData<T> data)
        {
            return data != null;
        }
        
        public static bool operator ==(PoolData<T> a, PoolData<T> b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;

            return EqualityComparer<T>.Default.Equals(a.Component, b.Component);
        }

        public static bool operator !=(PoolData<T> a, PoolData<T> b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj is PoolData<T> other)
                return EqualityComparer<T>.Default.Equals(Component, other.Component);

            return false;
        }

        public override int GetHashCode()
        {
            return Component == null ? 0 : Component.GetHashCode();
        }
    }
}