using UnityEngine;

namespace Systems.Pool
{
    public interface IPool
    {
        public void InitializeRandom(int amount, int offset = 0);
        public void InitializeAll(int offset = 0);
        public GameObject GetRandom(int offset = 0);
        public GameObject GetObject(GameObject prefab, int offset = 0);
        public void Return(GameObject obj);
        public void Clear();
    }
}