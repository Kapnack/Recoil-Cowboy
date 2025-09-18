using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Systems.Pool
{
    public class ObjectPoolManager : MonoBehaviour
    {
        private static readonly List<PooledObjectInfo> PooledObjectInfos = new();

        public enum PoolType
        {
            Enemies,
            Bullets,
            VFX,
            None
        }

        private static GameObject _emptyPoolObj;
        private static GameObject _enemiesPool;
        private static GameObject _bulletsPool;
        private static GameObject _vfxPoolObj;
    
        private void Awake()
        {
            SetUpEmpties();
        }

        private void SetUpEmpties()
        {
            _emptyPoolObj = new GameObject("Pooled Objects");
        
            _enemiesPool = new GameObject("Enemies Pool");
            _enemiesPool.transform.SetParent(_emptyPoolObj.transform);
        
            _bulletsPool =  new GameObject("Bullets Pool");
            _bulletsPool.transform.SetParent(_emptyPoolObj.transform);
        
            _vfxPoolObj = new GameObject("VFX Pool");
            _vfxPoolObj.transform.SetParent(_emptyPoolObj.transform);
        }
    
        private static GameObject SpawnObject(GameObject gameObject, Vector3 position, Quaternion rotation, PoolType poolType = PoolType.None)
        {
            var pool = PooledObjectInfos.Find(x => x.Name == gameObject.name);

            if (pool == null)
            {
                pool = new() { Name = gameObject.name };
            
                PooledObjectInfos.Add(pool);
            }

            var spawnableObj = pool.InactiveObjects.FirstOrDefault();

            if (!spawnableObj)
            {
                var parent = SetParent(poolType);
                spawnableObj = Instantiate(gameObject, position, rotation);

                if (parent != null)
                {
                    spawnableObj.transform.SetParent(parent.transform);
                }
            }
            else
            {
                spawnableObj.transform.position = position;
                spawnableObj.transform.rotation = rotation;
                pool.InactiveObjects.Remove(spawnableObj);
                spawnableObj.SetActive(true);
            }
        
            return spawnableObj;
        }

        private static void ReturnObjectToPool(GameObject obj)
        {
            var newName = obj.name.Replace("(Clone)", "");
            var pool = PooledObjectInfos.Find(x => x.Name == newName);

            if (pool == null)
            {
                Debug.LogError($"Pool \"{newName}\" not found");
                return;
            }
        
            obj.SetActive(false);
            pool.InactiveObjects.Add(obj);
        }

        private static GameObject SetParent(PoolType parent)
        {
            switch (parent)
            {
                case PoolType.Enemies:
                    return _enemiesPool;

                case PoolType.Bullets:
                    return _bulletsPool;

                case PoolType.VFX:
                    return _vfxPoolObj;

                default:
                    return null;
            }
        }
    }
}