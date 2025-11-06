using System.Collections.Generic;
using UnityEngine;

namespace Systems.Pool
{
    public class Pool<T> : IPool<PoolData<T>>
    {
        private readonly List<GameObject> _prefabs;
        private readonly Queue<PoolData<T>> _objects = new();

        private readonly Transform _idleObjFolder;
        private readonly Transform _activeObjFolder;

        public Pool(List<GameObject> prefabs, Transform folder, bool madeActiveFolder = true)
        {
            _prefabs = prefabs;

            _idleObjFolder = new GameObject("Pooled Objects").transform;
            _idleObjFolder.SetParent(folder);

            if(!madeActiveFolder)
                return;
            
            _activeObjFolder = new GameObject("Active Objects").transform;
            _activeObjFolder.SetParent(folder);
        }

        public Pool(GameObject prefab, Transform folder, bool madeActiveFolder = true)
        {
            _prefabs = new List<GameObject> { prefab };

            _idleObjFolder = new GameObject("Pool Objects").transform;
            _idleObjFolder.SetParent(folder);

            if(!madeActiveFolder)
                return;
            
            _activeObjFolder = new GameObject("Active Pool Objects").transform;
            _activeObjFolder.SetParent(folder);
        }

        public void InitializeRandom(int amount, int offset = 0)
        {
            for (int i = 0; i < amount; i++)
            {
                int index = Random.Range(offset, _prefabs.Count);

                AddedGameObject(_prefabs[index]);
            }
        }

        public void InitializeAll(int repeat = 0, int offset = 0)
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
                data.Obj.transform.SetParent(_activeObjFolder ? _activeObjFolder : null);
            }

            return data;
        }

        public void Return(PoolData<T> data)
        {
            if (!data)
                return;

            if (data.Component.Equals(null) || !data.Obj)
                return;

            data.Obj.SetActive(false);
            data.Obj.transform.SetParent(_idleObjFolder);

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
            GameObject obj = Object.Instantiate(prefab, _idleObjFolder);
            obj.SetActive(false);

            PoolData<T> poolData = new(obj);

            _objects.Enqueue(poolData);
        }

        private void SetComponent()
        {
        }
    }
}