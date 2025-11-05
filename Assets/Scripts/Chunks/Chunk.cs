using System;
using System.Collections;
using System.Collections.Generic;
using Characters.EnemySRC;
using Systems;
using Systems.Pool;
using Systems.TagClassGenerator;
using UnityEngine;

namespace Chunks
{
    [RequireComponent(typeof(BoxCollider))]
    public class Chunk : MonoBehaviour, IChunk
    {
        [SerializeField] private Transform chunkLimitTop;
        [SerializeField] private List<GameObject> spawnPoints;
        private readonly List<PoolData<IEnemy>> _spawnedEnemies = new();

        private IObjectPool<IEnemy> _objectPool;

        public Transform ChunkLimitTop => chunkLimitTop;

        public event Action LimitPass;

        private void Awake()
        {
            _objectPool = ServiceProvider.GetService<IObjectPool<IEnemy>>();

            BoxCollider boxCollider = GetComponent<BoxCollider>();

            boxCollider.isTrigger = true;
        }

        public void SetUp()
        {
            StartCoroutine(SetUpWait());
        }

        private IEnumerator SetUpWait()
        {
            yield return new WaitForEndOfFrame();
            
            foreach (GameObject spawnPoint in spawnPoints)
            {
                if (!spawnPoint)
                    continue;

                PoolData<IEnemy> enemy = _objectPool.Get();

                enemy.Obj.transform.position = spawnPoint.transform.position;
                enemy.Obj.transform.rotation = spawnPoint.transform.rotation;
                enemy.Component.SetUp(() => ReturnEnemy(enemy));

                _spawnedEnemies.Add(enemy);
            }
        }
        
        private void OnDisable()
        {
            foreach (PoolData<IEnemy> enemy in _spawnedEnemies)
                _objectPool.Return(enemy);

            _spawnedEnemies.Clear();
        }

        private void ReturnEnemy(PoolData<IEnemy> enemy)
        {
            _objectPool.Return(enemy);
            _spawnedEnemies.Remove(enemy);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tags.Player))
                LimitPass?.Invoke();
        }

        public List<PoolData<IEnemy>> GetList() => _spawnedEnemies;
    }

    public interface IChunk
    {
        public List<PoolData<IEnemy>> GetList();
    }
}