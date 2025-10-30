using System.Collections.Generic;
using UnityEngine;

namespace Systems.Pool
{
    public class Pool<T> : IPool<PoolData<T>>
    {
        private readonly List<GameObject> _prefabs;
        private readonly Queue<PoolData<T>> _objects = new();

        private readonly Transform _folder;

        public Pool(List<GameObject> prefabs, Transform folder)
        {
            _prefabs = prefabs;

            _folder = new GameObject("Pool Objects").transform;
            _folder.SetParent(folder);
        }

        public void InitializeRandom(int amount, int offset = 0)
        {
            for (int i = 0; i < amount; i++)
            {
                int index = Random.Range(offset, _prefabs.Count);

                AddedGameObject(_prefabs[index]);
            }
        }

        public void InitializeAll(int offset = 0)
        {
            for (int i = offset; i < _prefabs.Count; i++)
            {
                AddedGameObject(_prefabs[i]);
            }
        }

        public PoolData<T> Get(int offset = 0)
        {
            if (_objects.Count == 0)
                AddedGameObject(_prefabs[Random.Range(offset, _prefabs.Count)]);

            PoolData<T> obj = Release();

            return obj;
        }

        private PoolData<T> Release()
        {
            PoolData<T> data = _objects.Dequeue();

            if (data)
            {
                data.Obj.SetActive(true);
                data.Obj.transform.SetParent(null);
            }

            return data;
        }

        public void Return(PoolData<T> data)
        {
            if (!data)
                return;

            if (data.Component.Equals(null)|| !data.Obj)
                return;

            
            
            data.Obj.SetActive(false);
            data.Obj.transform.SetParent(_folder);

            _objects.Enqueue(data);
        }

        public void Clear()
        {
            foreach (PoolData<T> data in _objects)
                Object.Destroy(data.Obj);

            _objects.Clear();
        }

        private void AddedGameObject(GameObject prefab)
        {
            GameObject obj = Object.Instantiate(prefab, _folder);
            obj.SetActive(false);

            PoolData<T> poolData = new(obj);

            _objects.Enqueue(poolData);
        }

        private void SetComponent()
        {
            
        }
    }
}