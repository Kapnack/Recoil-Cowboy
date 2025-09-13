using ScriptableObjects;
using UnityEngine;

namespace Characters.EnemySRC
{
    public abstract class Enemy : MonoBehaviour, IHealthSystem
    {
        [SerializeField] private EnemyConfig _config;

        public void ReceiveDamage()
        {
            //TODO: Deactivate the Enemy to be reused by the Pool.
        }
    }
}