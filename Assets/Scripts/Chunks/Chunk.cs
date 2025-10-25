using System;
using System.Collections.Generic;
using Systems;
using Systems.TagClassGenerator;
using UnityEngine;

namespace Chunks
{
    [RequireComponent(typeof(BoxCollider))]
    public class Chunk : MonoBehaviour
    {
        [SerializeField] private Transform chunkLimitTop;
        [SerializeField] private List<GameObject> spawnPoints;
        private readonly List<GameObject> _spawnedEnemies = new();

        private IEnemyPool _enemyPool;
        
        public Transform ChunkLimitTop => chunkLimitTop;

        public event Action LimitPass;
        
        private void Awake()
        {
            _enemyPool = ServiceProvider.GetService<IEnemyPool>();
            
            BoxCollider boxCollider = GetComponent<BoxCollider>();

            boxCollider.isTrigger = true;
        }

        public void SetUp()
        {
            foreach (GameObject spawnPoint in spawnPoints)
            {
                GameObject enemy = _enemyPool.Get();
                
                enemy.transform.position = spawnPoint.transform.position;
                
                _spawnedEnemies.Add(enemy);
            }
        }

        private void OnDisable()
        {
            foreach (GameObject enemy in _spawnedEnemies)
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