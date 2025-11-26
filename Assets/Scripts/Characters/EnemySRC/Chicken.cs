using System;
using Characters.PlayerSRC;
using Particle;
using ScriptableObjects;
using Systems;
using Systems.Pool;
using Systems.TagClassGenerator;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Characters.EnemySRC
{
    public class Chicken : Enemy
    {
        [SerializeField] private ChickenConfig config;
        private Vector3 _raycastOrigin;

        private bool _alreadyRotated;

        private float _jumpTimer;

        private RaycastHit _hit;

        private IAnimate _animator;

        [SerializeField] private GameObject explosionParticle;
        [SerializeField] private GameObject deadParticle;
        private IObjectPool<ParticleController> _particlePool;

        private void Start()
        {
            _particlePool = ServiceProvider.GetService<IObjectPool<ParticleController>>();
        }

        private void OnEnable() => SetUp();

        public override void SetUp(Action action = null)
        {
            base.SetUp(action);
            SetJumpTimer();
            Rb.linearVelocity = Vector3.zero;
            _animator = GetComponent<IAnimate>();
        }

        private void FixedUpdate()
        {
            Movement();

            CheckForEnemies();
        }

        private void Movement()
        {
            _raycastOrigin = transform.position + config.RaycastOffSet * transform.right;

            if (!Physics.Raycast(_raycastOrigin, Vector3.down, out _hit, config.RaycastDistance))
            {
                if (_hit.collider && _hit.collider.CompareTag(Tags.Player))
                    return;

                if (!_alreadyRotated)
                {
                    Rotate();
                    _animator.ChangeAnimation(false);
                }
            }
            else if (Physics.Raycast(_raycastOrigin, transform.right, out _hit, config.RaycastDistance))
            {
                if (_hit.collider && _hit.collider.CompareTag(Tags.Player))
                    return;

                Rotate();
            }
            else
            {
                if (Rb.linearVelocity.sqrMagnitude < config.MaxVelocity * config.MaxVelocity)
                    Rb.AddForce(transform.right * config.MoveSpeed, ForceMode.Force);
                else
                    Rb.linearVelocity = transform.right * config.MaxVelocity;

                _alreadyRotated = false;
            }

            if (_jumpTimer < Time.time)
            {
                _animator.ChangeAnimation(true);
                Rb.AddForce(Vector3.up * config.JumpForce, ForceMode.Impulse);
                SetJumpTimer();
            }
        }

        private void SetJumpTimer() =>
            _jumpTimer = Time.time + Random.Range(config.JumpMinTimer, config.JumpMaxTimer);

        private void Rotate()
        {
            Rb.linearVelocity = new Vector3(0.0f, Rb.linearVelocity.y, 0.0f);

            Vector3 currentRotation = transform.eulerAngles;
            currentRotation.y += 180.0f;
            transform.rotation = Quaternion.Euler(currentRotation);

            _alreadyRotated = true;
        }

        private void CheckForEnemies()
        {
            Collider[] objects = Physics.OverlapSphere(transform.position, config.AreaOfSight);

            foreach (Collider obj in objects)
            {
                if (!obj.CompareTag(Tags.Player))
                    continue;

                Vector3 dir = (obj.transform.position - transform.position).normalized;

                if (Physics.Raycast(transform.position, dir, out _hit, config.AreaOfSight))
                {
                    if (!_hit.collider.CompareTag(Tags.Player))
                        return;

                    if (obj.transform.TryGetComponent<IHealthSystem>(out var healthSystem))
                    {
                        if (healthSystem is IPlayerHealthSystem playerHealthSystem)
                            if (playerHealthSystem.Invincible)
                                return;

                        healthSystem.ReceiveDamage();
                        SpawnParticle(explosionParticle);
                        ReceiveDamage();
                    }
                }
            }
        }

        public override void ReceiveDamage(Action action = null)
        {
            AkUnitySoundEngine.PostEvent("sfx_ChickenExp", gameObject);

            SpawnParticle(deadParticle);

            base.ReceiveDamage(action);
        }

        private async void SpawnParticle(GameObject prefab)
        {
            PoolData<ParticleController> particle = await _particlePool.Get(prefab);

            particle.Obj.transform.position = transform.position;
            particle.Component.SetUp(() => _particlePool.Return(particle));
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!config)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, config.AreaOfSight);

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(_raycastOrigin, Vector3.down * config.RaycastDistance);
            Gizmos.DrawRay(_raycastOrigin, transform.right * config.RaycastDistance);
        }


        private void OnValidate()
        {
            if (config == null)
                Debug.LogError($"_config is null in GameObject {gameObject.name}");
            else if (!config)
                Debug.LogError($"_config is not ChickenConfig in GameObject {gameObject.name}");
        }
#endif
    }
}