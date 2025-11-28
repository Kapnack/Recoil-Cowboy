using System;
using System.Collections;
using Characters.PlayerSRC;
using Particle;
using ScriptableObjects;
using Systems;
using Systems.Pool;
using Systems.TagClassGenerator;
using Unity.VisualScripting;
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
        private bool _wentToStartingPos;

        [SerializeField] private GameObject vultureBody;

        [SerializeField] private GameObject deadParticle;
        private IObjectPool<ParticleController> _particlePool;

        protected override void Awake()
        {
            base.Awake();
            vultureBody.transform.forward = transform.right;
        }

        private void Start()
        {
            _particlePool = ServiceProvider.GetService<IObjectPool<ParticleController>>();
        }

        private void OnEnable()
        {
            AkUnitySoundEngine.PostEvent("sfx_Crow_flight",gameObject);
        }
        private void OnDisable()
        {
            AkUnitySoundEngine.PostEvent("sfx_Crow_flight_stop", gameObject);
        }
        public override void SetUp(Action action = null)
        {
            base.SetUp(action);
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

                Vector3 direction = (_target.transform.position - transform.position).normalized;

                vultureBody.transform.forward = direction;
                Rb.AddForce(direction * (config.MoveSpeed * Time.fixedDeltaTime), ForceMode.VelocityChange);
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
                if (Rb.linearVelocity.sqrMagnitude < config.MaxVelocity * config.MaxVelocity)
                    Rb.AddForce(transform.right * config.MoveSpeed, ForceMode.Force);
                else
                    Rb.linearVelocity = transform.right * config.MaxVelocity;
            }
        }

        private void Rotate()
        {
            Rb.linearVelocity = new Vector3(0.0f, Rb.linearVelocity.y, 0.0f);

            Vector3 currentRotation = transform.eulerAngles;
            currentRotation.y += 180.0f;
            vultureBody.transform.forward = currentRotation;
            transform.rotation = Quaternion.Euler(currentRotation);
        }

        private IEnumerator BackToStartingPos()
        {
            Rb.linearVelocity = Vector3.zero;
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
            Collider[] objects = Physics.OverlapSphere(transform.position, config.AreaOfSight);

            foreach (Collider obj in objects)
            {
                if (!obj.CompareTag(Tags.Player))
                    continue;

                if (obj.transform.TryGetComponent(out IPlayerHealthSystem player))
                {
                    if (!player.Invincible)
                    {
                        Vector3 direction = (obj.transform.position - transform.position).normalized;

                        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, config.AreaOfSight))
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

        public override async void ReceiveDamage(Action action = null)
        {
            PoolData<ParticleController> particleGo = await _particlePool.Get(deadParticle);

            particleGo.Obj.transform.position = transform.position;
            particleGo.Component.SetUp(() => _particlePool.Return(particleGo));
            AkUnitySoundEngine.PostEvent("sfx_crow_death", gameObject);
            base.ReceiveDamage(action);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.TryGetComponent(out IHealthSystem healthSystem))
            {
                AkUnitySoundEngine.PostEvent("sfx_crow_hit", gameObject);
                healthSystem.ReceiveDamage();
            }
        }

        private void OnCollisionStay(Collision collision) => OnCollisionEnter(collision);

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!config)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, config.AreaOfSight);
        }
#endif
    }
}