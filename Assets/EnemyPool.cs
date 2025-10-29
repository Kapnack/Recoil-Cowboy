using System.Collections.Generic;
using Characters.EnemySRC;
using Systems;
using Systems.Pool;
using UnityEngine;

public class EnemyPool : MonoBehaviour, IEnemyPool<IEnemy>
{
    private Pool<IEnemy> _pool;
    [SerializeField] private List<GameObject> enemies;

    private void Awake()
    {
        _pool = new Pool<IEnemy>(enemies, transform);
        _pool.InitializeRandom(1);
        
        ServiceProvider.SetService<IEnemyPool<IEnemy>>(this, true);
    }

    public PoolData<IEnemy> Get() => _pool.Get();

    public void Return(PoolData<IEnemy> enemy) => _pool.Return(enemy);
}

public interface IEnemyPool<T>
{
    public PoolData<T> Get();
    void Return(PoolData<IEnemy> enemy);
}