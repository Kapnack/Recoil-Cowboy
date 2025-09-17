using UnityEngine;

namespace Characters.EnemySRC
{
    public class Cactus : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            OnCollisionStay(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.transform.TryGetComponent<IHealthSystem>(out var healthSystem))
                healthSystem.ReceiveDamage();
        }
    }
}