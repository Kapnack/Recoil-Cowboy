using System.Collections;
using Characters.PlayerSRC;
using ScriptableObjects;
using Systems.TagClassGenerator;
using UnityEngine;

namespace Characters.EnemySRC
{
    public class Vulture : Enemy
    {
        [SerializeField] private VultureConfig config;
        private Vector3 _spawnPosition;

        private Transform _target;

        private Coroutine _backToStartCoroutine;
        private Vector3 _currentVelocity;

        protected override void Awake()
        {
            base.Awake();

            _spawnPosition = transform.position;
        }

        private void FixedUpdate()
        {
            _target = FindNearestTarget();

            if (_target)
            {
                if (_backToStartCoroutine != null)
                {
                    StopCoroutine(_backToStartCoroutine);
                    _backToStartCoroutine = null;
                }

                var direction = (_target.transform.position - transform.position).normalized;

                _rb.AddForce(direction * (config.MoveSpeed * Time.fixedDeltaTime), ForceMode.VelocityChange);
            }
            else
            {
                _backToStartCoroutine ??= StartCoroutine(BackToStartingPos());
            }
        }

        private IEnumerator BackToStartingPos()
        {
            _rb.linearVelocity = Vector3.zero;
            _currentVelocity = Vector3.zero;

            while ((_spawnPosition - transform.position).sqrMagnitude > Mathf.Epsilon)
            {
                transform.position =
                    Vector3.SmoothDamp(transform.position, _spawnPosition, ref _currentVelocity, config.SmoothBackTime);
                yield return null;
            }

            _backToStartCoroutine = null;
        }

        private Transform FindNearestTarget()
        {
            var objects = Physics.OverlapSphere(transform.position, config.AreaOfSight);

            foreach (var obj in objects)
            {
                if (!obj.CompareTag(Tags.Player))
                    continue;

                if (obj.transform.TryGetComponent<IPlayerHealthSystem>(out var player))
                {
                    if (!player.Invincible)
                    {
                        var direction = (obj.transform.position - transform.position).normalized;

                        if (Physics.Raycast(transform.position, direction, out var hit, config.AreaOfSight))
                        {
                            Debug.DrawLine(transform.position, hit.transform.position, Color.red);

                            if (hit.transform.gameObject == obj.gameObject)
                                return obj.transform;
                        }
                    }
                }
            }

            return null;
        }

        private void OnDrawGizmos()
        {
            if (!config)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, config.AreaOfSight);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.TryGetComponent<IHealthSystem>(out var healthSystem))
                healthSystem.ReceiveDamage();
        }
    }
}