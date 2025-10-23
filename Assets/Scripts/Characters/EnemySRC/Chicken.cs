using Characters.PlayerSRC;
using ScriptableObjects;
using Systems.TagClassGenerator;
using UnityEngine;
using UnityEngine.Serialization;

namespace Characters.EnemySRC
{
    public class Chicken : Enemy
    {
        [SerializeField] private ChickenConfig chickenConfig;
        private Vector3 _raycastOrigin;

        private bool _alreadyRotated;

        private float _jumpTimer;

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
            _raycastOrigin = transform.position + chickenConfig.RaycastOffSet * transform.right;

            if (!Physics.Raycast(_raycastOrigin, Vector3.down, chickenConfig.RaycastDistance))
            {
                if (!_alreadyRotated)
                    Rotate();
            }
            else if (Physics.Raycast(_raycastOrigin, transform.right, chickenConfig.RaycastDistance))
            {
                Rotate();
            }
            else
            {
                if (_rb.linearVelocity.sqrMagnitude < chickenConfig.MaxVelocity * chickenConfig.MaxVelocity)
                    _rb.AddForce(transform.right * chickenConfig.MoveSpeed, ForceMode.Force);
                else
                    _rb.linearVelocity = transform.right * chickenConfig.MaxVelocity;

                _alreadyRotated = false;
            }

            if (_jumpTimer < Time.time)
            {
                _rb.AddForce(Vector3.up * chickenConfig.JumpForce, ForceMode.Impulse);
                SetJumpTimer();
            }
        }

        private void SetJumpTimer() => _jumpTimer = Time.time + Random.Range(chickenConfig.JumpMinTimer, chickenConfig.JumpMaxTimer);

        private void Rotate()
        {
            _rb.linearVelocity = new Vector3(0.0f, _rb.linearVelocity.y, 0.0f);

            var currentRotation = transform.eulerAngles;
            currentRotation.y += 180.0f;
            transform.rotation = Quaternion.Euler(currentRotation);

            _alreadyRotated = true;
        }

        private void CheckForEnemies()
        {
            var objects = Physics.OverlapSphere(transform.position, chickenConfig.AreaOfSight);

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
                    if (Physics.Raycast(transform.position, direction, out var hit, chickenConfig.AreaOfSight))
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
            if (!chickenConfig)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chickenConfig.AreaOfSight);

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(_raycastOrigin, Vector3.down * chickenConfig.RaycastDistance);
            Gizmos.DrawRay(_raycastOrigin, transform.right * chickenConfig.RaycastDistance);
        }
        public override void ReceiveDamage()
        {
            base.ReceiveDamage();
            AkUnitySoundEngine.PostEvent("sfx_ChickenExp", gameObject);
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (chickenConfig == null)
                Debug.LogError($"_config is null in GameObject {gameObject.name}");
            else if (!(chickenConfig as ChickenConfig))
                Debug.LogError($"_config is not ChickenConfig in GameObject {gameObject.name}");
        }
#endif
    }
}