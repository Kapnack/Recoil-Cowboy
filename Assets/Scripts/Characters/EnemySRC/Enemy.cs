using System;
using UnityEngine;

namespace Characters.EnemySRC
{
    public abstract class Enemy : Character, IEnemyHealthSystem, IEnemy
    {
        protected static readonly LayerMask LayerMask = 64;
        protected event Action _killed;

        public virtual void SetUp(Action action = null)
        {
            _killed = null;

            if (action != null)
                _killed = action;
        }

        public virtual void ReceiveDamage(Action action = null)
        {
            action?.Invoke();
            _killed?.Invoke();
        }
    }
}