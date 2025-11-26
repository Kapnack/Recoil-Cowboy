using System;
using UnityEngine;

namespace Characters.EnemySRC
{
    public abstract class Enemy : Character, IEnemyHealthSystem, IEnemy
    {
        protected event Action _killed;

        public virtual void SetUp(Action action = null)
        {
            ResetState();
            _killed += action;
        }

        private void ResetState()
        {
            Rb.linearVelocity = Vector3.zero;
            Rb.angularVelocity = Vector3.zero;
            
            StopAllCoroutines();
            
            _killed = ResetState;
        }
        
        public virtual void ReceiveDamage(Action action = null)
        {
            action?.Invoke();
            _killed?.Invoke();
        }
    }
}