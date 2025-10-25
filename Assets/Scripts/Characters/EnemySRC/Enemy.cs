using System;

namespace Characters.EnemySRC
{
    public abstract class Enemy : Character, IHealthSystem
    {
        protected abstract void SetUp();

        public virtual void ReceiveDamage()
        {
            gameObject.SetActive(false);
        }
    }
}