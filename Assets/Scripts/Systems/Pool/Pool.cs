using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Systems.Pool
{
    public class Pool<T> : IPool<PoolData<T>>
    {
        private readonly List<GameObject> _prefabs;
        private readonly List<PoolData<T>> _enqueuedObjects = new();

        private readonly Transform _idleObjFolder;
        private readonly Transform _activeObjFolder;

        public Pool(List<GameObject> prefabs, Transform folder, bool madeActiveFolder = true)
        {
            _prefabs = prefabs;

            _idleObjFolder = new GameObject("Pooled Objects").transform;
            _idleObjFolder.SetParent(folder);

            if (!madeActiveFolder)
                return;

            _activeObjFolder = new GameObject("Active Objects").transform;
            _activeObjFolder.SetParent(folder);
        }

        public Pool(GameObject prefab, Transform folder, bool madeActiveFolder = true)
        {
            _prefabs = new List<GameObject> { prefab };

            _idleObjFolder = new GameObject("Pool Objects").transform;
            _idleObjFolder.SetParent(folder);

            if (!madeActiveFolder)
                return;

            _activeObjFolder = new GameObject("Active Pool Objects").transform;
            _activeObjFolder.SetParent(folder);
        }

        public async void InitializeRandom(int amount, int offset = 0)
        {
            for (int i = 0; i < amount; i++)
            {
                int index = Random.Range(offset, _prefabs.Count);

                _enqueuedObjects.Add(await AddedGameObject(_prefabs[index]));
            }
        }

        public async void InitializeAll(int repeat = 1, int offset = 0)
        {
            for (int i = 0; i < repeat; i++)
            {
                for (int j = offset; j < _prefabs.Count; j++)
                {
                    _enqueuedObjects.Add(await AddedGameObject(_prefabs[j]));
                }
            }
        }

        public async Task<PoolData<T>> Get(int offset = 0)
        {
            if (_enqueuedObjects.Count == 0)
                return Release(await AddedGameObject(_prefabs[Random.Range(offset, _prefabs.Count)]));

            return Release();
        }

        public async Task<PoolData<T>> Get(GameObject prefab)
        {
            if (_enqueuedObjects.Count == 0)
                return Release(await AddedGameObject(prefab));

            return await Release(prefab);
        }

        private PoolData<T> Release(PoolData<T> data = null)
        {
            data ??= _enqueuedObjects[0];

            _enqueuedObjects.Remove(data);

            data.Obj.SetActive(true);
            data.Obj.transform.SetParent(_activeObjFolder ? _activeObjFolder : null);

            return data;
        }

        private async Task<PoolData<T>> Release(GameObject prefab)
        {
            PoolData<T> data = _enqueuedObjects.FirstOrDefault(x => !x.Obj.activeSelf && x.PrefabSource == prefab);

            if (data == null)
                data = await AddedGameObject(prefab);

            data.Obj.SetActive(true);
            data.Obj.transform.SetParent(_activeObjFolder ? _activeObjFolder : null);

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

            _enqueuedObjects.Add(data);
        }

        public void Clear()
        {
            foreach (PoolData<T> data in _enqueuedObjects)
                Object.Destroy(data.Obj);

            _enqueuedObjects.Clear();
        }

        private async Task<PoolData<T>> AddedGameObject(GameObject prefab)
        {
            if (!prefab)
            {
                return null;
            }
            
            AsyncInstantiateOperation<GameObject> operation = Object.InstantiateAsync(prefab, _idleObjFolder);

            await operation;

            GameObject obj = operation.Result[0];

            obj.SetActive(false);

            return new PoolData<T>(obj, prefab);
        }
    }
}