using System;

namespace Characters.EnemySRC
{
    public abstract class Enemy : Character, IHealthSystem, IEnemy
    {
        public abstract void SetUp();

        public virtual void ReceiveDamage()
        {
            gameObject.SetActive(false);
        }
    }
}