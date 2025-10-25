using System;
using System.Collections.Generic;
using Characters.EnemySRC;
using Systems;
using Systems.Pool;
using Systems.TagClassGenerator;
using UnityEngine;

namespace Chunks
{
    [RequireComponent(typeof(BoxCollider))]
    public class Chunk : MonoBehaviour
    {
        [SerializeField] private Transform chunkLimitTop;
        [SerializeField] private List<GameObject> spawnPoints;
        private readonly List<PoolData<IEnemy>> _spawnedEnemies = new();

        private IEnemyPool<IEnemy> _enemyPool;
        
        public Transform ChunkLimitTop => chunkLimitTop;

        public event Action LimitPass;
        
        private void Awake()
        {
            _enemyPool = ServiceProvider.GetService<IEnemyPool<IEnemy>>();
            
            BoxCollider boxCollider = GetComponent<BoxCollider>();

            boxCollider.isTrigger = true;
        }

        public void SetUp()
        {
            foreach (GameObject spawnPoint in spawnPoints)
            {
                PoolData<IEnemy> enemy = _enemyPool.Get();
                
                enemy.Obj.transform.position = spawnPoint.transform.position;
                enemy.Component.SetUp();
                
                _spawnedEnemies.Add(enemy);
            }
        }

        private void OnDisable()
        {
            foreach (PoolData<IEnemy> enemy in _spawnedEnemies)
            {
                _enemyPool.Return(enemy);
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag(Tags.Player))
                LimitPass?.Invoke();
        }
    }
}