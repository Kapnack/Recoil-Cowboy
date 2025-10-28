using System.Collections.Generic;
using Characters.EnemySRC;
using Systems.Pool;
using Systems.TagClassGenerator;
using UnityEngine;

namespace Chunks
{
    public class EnemyOwnership : MonoBehaviour
    {
        private List<PoolData<IEnemy>> _ownedEnemies;

        private void Awake()
        {
            _ownedEnemies = GetComponent<IChunk>().GetList();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tags.Enemy))
            {
                PoolData<IEnemy> poolData = _ownedEnemies.Find(e => e.Obj == other.gameObject);
                _ownedEnemies.Add(poolData);
                poolData.Component.SetUp();
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Tags.Enemy))
            {
                PoolData<IEnemy> poolData = _ownedEnemies.Find(e => e.Obj == other.gameObject);
                _ownedEnemies.Remove(poolData);
            }
        }
    }
}