using UnityEngine;

namespace Characters.EnemySRC
{
    public class Cactus : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.TryGetComponent<IHealthSystem>(out var healthSystem))
                healthSystem.ReceiveDamage();
        }

        private void OnCollisionStay(Collision collision) => OnCollisionEnter(collision);
    }
}