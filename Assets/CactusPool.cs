using Characters.EnemySRC;
using Systems.Pool;
using UnityEngine;

public class CactusPool : MonoBehaviour, IObjectPool<Cactus>
{
    private Pool<IEnemy> _pool;
    [SerializeField] private GameObject enemies;
    public PoolData<Cactus> Get()
    {
        throw new System.NotImplementedException();
    }

    public void Return(PoolData<IEnemy> enemy)
    {
        throw new System.NotImplementedException();
    }
}