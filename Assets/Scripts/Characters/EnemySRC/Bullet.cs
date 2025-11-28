using UnityEngine;

namespace Characters.EnemySRC
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Bullet : MonoBehaviour
    {
        private Character _owner;
        private Rigidbody _rb;
        [SerializeField] private float secondsToExist = 5.0f;
        private float _timeToDestroy;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (Time.time > _timeToDestroy)
                Destroy(gameObject);
        }

        public void Launch(Character owner, Vector3 spawnPosition, Vector3 dir, float speed)
        {
            _owner = owner;
            transform.position = spawnPosition;

            _rb.AddForce(dir * speed, ForceMode.Impulse);
            
            _timeToDestroy = Time.time + secondsToExist;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_owner && collision.gameObject == _owner.gameObject)
                return;

            if (collision.transform.TryGetComponent(out IHealthSystem healthSystem))
            {
                healthSystem.ReceiveDamage();
            }

            Destroy(gameObject);
        }
    }
}