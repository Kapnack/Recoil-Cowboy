using System;
using UnityEngine;

namespace Characters.EnemySRC
{
    public class Cactus : MonoBehaviour, IEnemy
    {
        private void OnTriggerEnter(Collider collision)
        {
            if (collision.TryGetComponent<IHealthSystem>(out var healthSystem))
                healthSystem.ReceiveDamage();
        }

        private void OnTriggerStay(Collider collision) => OnTriggerEnter(collision);
        
        public void SetUp(Action action = null)
        {
            
        }
    }
}