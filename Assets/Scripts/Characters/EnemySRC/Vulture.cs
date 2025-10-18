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
        private Vector3 _raycastOrigin;
        private RaycastHit _hit;
        private bool _alreadyRotated;
        private bool _wentToStartingPos;

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
                _wentToStartingPos = false;

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
                if (!_wentToStartingPos)
                    _backToStartCoroutine ??= StartCoroutine(BackToStartingPos());
                else
                    Movement();
            }
        }

        private void Movement()
        {
            _raycastOrigin = transform.position + 1 * transform.right;

            if (Physics.Raycast(_raycastOrigin, transform.right, out _hit, 1))
            {
                if (_hit.collider && _hit.collider.CompareTag(Tags.Player))
                    return;

                Rotate();
            }
            else
            {
                if (_rb.linearVelocity.sqrMagnitude < config.MaxVelocity * config.MaxVelocity)
                    _rb.AddForce(transform.right * config.MoveSpeed, ForceMode.Force);
                else
                    _rb.linearVelocity = transform.right * config.MaxVelocity;

                _alreadyRotated = false;
            }
        }

        private void Rotate()
        {
            _rb.linearVelocity = new Vector3(0.0f, _rb.linearVelocity.y, 0.0f);

            var currentRotation = transform.eulerAngles;
            currentRotation.y += 180.0f;
            transform.rotation = Quaternion.Euler(currentRotation);

            _alreadyRotated = true;
        }

        private IEnumerator BackToStartingPos()
        {
            _rb.linearVelocity = Vector3.zero;
            _currentVelocity = Vector3.zero;

            while ((_spawnPosition - transform.position).sqrMagnitude > 0.1f * 0.1f)
            {
                transform.position =
                    Vector3.SmoothDamp(transform.position, _spawnPosition, ref _currentVelocity, config.SmoothBackTime);
                yield return null;
            }

            _backToStartCoroutine = null;

            _wentToStartingPos = true;
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