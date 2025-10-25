using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Systems.Pool
{
    public class Pool : IPool
    {
        private readonly List<GameObject> _prefabs;
        private readonly Queue<GameObject> _objects = new();

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

                GameObject instance = Object.Instantiate(_prefabs[index], _folder);
                instance.SetActive(false);

                _objects.Enqueue(instance);
            }
        }

        public void InitializeAll(int offset = 0)
        {
            for (int i = offset; i < _prefabs.Count; i++)
            {
                GameObject instance = Object.Instantiate(_prefabs[i], _folder);
                instance.SetActive(false);

                _objects.Enqueue(instance);
            }
        }

        public GameObject GetRandom(int offset = 0)
        {
            int index = Random.Range(offset, _prefabs.Count);

            GameObject obj;

            if (_objects.Count == 0)
            {
                AddedGameObject(_prefabs[index]);
                obj = _objects.Dequeue();
                obj.SetActive(true);
                obj.transform.SetParent(null);
                return obj;
            }

            obj = _objects.Dequeue();

            if (!obj)
            {
                InitializeRandom(offset, 1);
                obj = _objects.Dequeue();
            }

            obj.SetActive(true);
            obj.transform.SetParent(null);

            return obj;
        }

        public GameObject GetObject(GameObject prefab, int offset = 0)
        {
            GameObject obj;

            if (_objects.Count == 0)
            {
                AddedGameObject(prefab);
                obj = _objects.Dequeue();
                obj.SetActive(true);
                obj.transform.SetParent(null);
                return obj;
            }

            obj = _objects.Dequeue();

            if (!obj)
            {
                InitializeRandom(offset, 1);
                obj = _objects.Dequeue();
            }

            obj.SetActive(true);
            obj.transform.SetParent(null);
            
            return obj;
        }

        public void Return(GameObject obj)
        {
            if (!obj)
                return;

            obj.SetActive(false);
            obj.transform.SetParent(_folder);
            
            _objects.Enqueue(obj);
        }

        public void Clear()
        {
            foreach (GameObject obj in _objects)
                Object.Destroy(obj);

            _objects.Clear();
        }

        private void AddedGameObject(GameObject prefab)
        {
            GameObject go = Object.Instantiate(prefab, _folder);
            go.SetActive(false);
            _objects.Enqueue(go);
        }
    }
}