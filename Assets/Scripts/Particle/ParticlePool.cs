using System.Threading.Tasks;
using Systems.Pool;
using UnityEngine;

namespace Particle
{
    public class ParticlePool : MonoBehaviour, IObjectPool<ParticleController>
    {
        [SerializeField] private GameObject particlePrefab;
        private Pool<ParticleController> _pool;

        private void Awake()
        {
            _pool = new Pool<ParticleController>(particlePrefab, transform, false);

            _pool.InitializeAll(3);
        }

        public Task<PoolData<ParticleController>> Get()
        {
            return _pool.Get();
        }

        public void Return(PoolData<ParticleController> obj)
        {
            _pool.Return(obj);
        }
    }
}