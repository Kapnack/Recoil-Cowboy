using System;
using Characters.PlayerSRC;
using ScriptableObjects;
using Systems.TagClassGenerator;
using UnityEngine;
using UnityEngine.Serialization;
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

        protected override void Awake()
        {
            base.Awake();
            SetJumpTimer();
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
                    Rotate();
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
                Rb.AddForce(Vector3.up * config.JumpForce, ForceMode.Impulse);
                SetJumpTimer();
            }
        }

        private void SetJumpTimer() =>
            _jumpTimer = Time.time + Random.Range(config.JumpMinTimer, config.JumpMaxTimer);

        private void Rotate()
        {
            Rb.linearVelocity = new Vector3(0.0f, Rb.linearVelocity.y, 0.0f);

            var currentRotation = transform.eulerAngles;
            currentRotation.y += 180.0f;
            transform.rotation = Quaternion.Euler(currentRotation);

            _alreadyRotated = true;
        }

        private void CheckForEnemies()
        {
            var objects = Physics.OverlapSphere(transform.position, config.AreaOfSight);

            foreach (var obj in objects)
            {
                if (!obj.CompareTag(Tags.Player))
                    continue;

                if (obj.transform.TryGetComponent<IHealthSystem>(out var healthSystem))
                {
                    if (healthSystem is IPlayerHealthSystem playerHealthSystem)
                    {
                        if (playerHealthSystem.Invincible)
                            return;
                    }

                    var direction = (obj.transform.position - transform.position).normalized;
                    if (Physics.Raycast(transform.position, direction, out var hit, config.AreaOfSight))
                    {
                        Debug.DrawLine(transform.position, hit.transform.position, Color.red);
                        healthSystem.ReceiveDamage();
                        ReceiveDamage();
                    }
                }
            }
        }

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
        public override void ReceiveDamage()
        {
            base.ReceiveDamage();
            AkUnitySoundEngine.PostEvent("sfx_ChickenExp", gameObject);
        }
#if UNITY_EDITOR
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