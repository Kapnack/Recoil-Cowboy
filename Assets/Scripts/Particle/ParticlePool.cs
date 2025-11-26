using System.Collections.Generic;
using System.Threading.Tasks;
using Systems;
using Systems.Pool;
using UnityEngine;

namespace Particle
{
    public class ParticlePool : MonoBehaviour, IObjectPool<ParticleController>
    {
        [SerializeField] private List<GameObject> particlePrefab;
        private Pool<ParticleController> _pool;

        private void Awake()
        {
            ServiceProvider.SetService<IObjectPool<ParticleController>>(this, true);
            
            _pool = new Pool<ParticleController>(particlePrefab, transform, false);

            _pool.InitializeAll();
        }

        public Task<PoolData<ParticleController>> Get() => _pool.Get();

        public Task<PoolData<ParticleController>> Get(GameObject prefab) => _pool.Get(prefab);

        public void Return(PoolData<ParticleController> obj) => _pool.Return(obj);
    }
}