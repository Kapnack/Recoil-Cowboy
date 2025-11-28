using System;

namespace Characters
{
    public interface IHealthSystem
    {
        public void ReceiveDamage(Action action = null);
        public void InstantDead();
    }
}