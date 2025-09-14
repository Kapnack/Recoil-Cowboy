using UnityEngine;

namespace Characters.EnemySRC
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class Bullet : MonoBehaviour
    {
        private Character _owner;
        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void Launch(Character owner, Vector3 spawnPosition, float speed)
        {
            _owner = owner;
            transform.position = spawnPosition;

            _rb.AddForce(transform.right * speed, ForceMode.Impulse);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject == _owner.gameObject)
                return;

            if (other.transform.TryGetComponent<IHealthSystem>(out var healthSystem))
            {
                healthSystem.ReceiveDamage();
            }

            Destroy(gameObject);
        }
    }
}