using System;
using UnityEngine;

namespace Characters.EnemySRC
{
    public class Cactus : MonoBehaviour, IEnemy
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.TryGetComponent<IHealthSystem>(out var healthSystem))
                healthSystem.ReceiveDamage();
        }

        private void OnCollisionStay(Collision collision) => OnCollisionEnter(collision);
        
        public void SetUp(Action action = null)
        {
            
        }
    }
}