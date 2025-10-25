using System.Collections.Generic;
using Systems;
using Systems.Pool;
using UnityEngine;

public class EnemyPool : MonoBehaviour, IEnemyPool
{
    private IPool _pool;
    [SerializeField] private List<GameObject> enemies;

    private void Awake()
    {
        _pool = new Pool(enemies, transform);
        _pool.InitializeRandom(1);
        
        ServiceProvider.SetService<IEnemyPool>(this, true);
    }

    public GameObject Get() => _pool.GetRandom();
    public void Return(GameObject enemy) => _pool.Return(enemy);
}

public interface IEnemyPool
{
    public GameObject Get();
    void Return(GameObject enemy);
}