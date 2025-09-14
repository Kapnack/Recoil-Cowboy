using ScriptableObjects;
using UnityEngine;

namespace Characters.EnemySRC
{
    public abstract class Enemy : Character, IHealthSystem
    {
        public virtual void ReceiveDamage()
        {
            //TODO: Deactivate the Enemy to be reused by the Pool.
        }
    }
}