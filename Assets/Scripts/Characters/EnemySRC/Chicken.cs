using Characters.PlayerSRC;
using ScriptableObjects;
using Systems.TagClassGenerator;
using UnityEngine;

namespace Characters.EnemySRC
{
    public class Chicken : Enemy
    {
        private ChickenConfig _chickenConfig;
        private Vector3 _raycastOrigin;
        
        private bool _alreadyRotated;

        protected override void Awake()
        {
            base.Awake();
            
            _chickenConfig = (ChickenConfig)_config;
        }

        private void FixedUpdate()
        {
            Movement();

            CheckForEnemies();
        }

        private void Movement()
        {
            _raycastOrigin = transform.position + _chickenConfig.RaycastOffSet * transform.right;

            if (!Physics.Raycast(_raycastOrigin, Vector3.down, _chickenConfig.RaycastDistance))
            {
                if (!_alreadyRotated)
                    Rotate();
            }
            else if (Physics.Raycast(_raycastOrigin, transform.right, _chickenConfig.RaycastDistance))
            {
                Rotate();
            }
            else
            {
                if (_rb.linearVelocity.sqrMagnitude < _chickenConfig.MaxVelocity * _chickenConfig.MaxVelocity)
                    _rb.AddForce(transform.right * _chickenConfig.MaxVelocity, ForceMode.Force);
                else
                    _rb.linearVelocity = transform.right * _chickenConfig.MaxVelocity;

                _alreadyRotated = false;
            }
        }

        private void Rotate()
        {
            _rb.linearVelocity = Vector3.zero;

            var currentRotation = transform.eulerAngles;
            currentRotation.y += 180.0f;
            transform.rotation = Quaternion.Euler(currentRotation);

            _alreadyRotated = true;
        }

        private void CheckForEnemies()
        {
            var objects = Physics.OverlapSphere(transform.position, _chickenConfig.AreaOfSight);

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
                    if (Physics.Raycast(transform.position, direction, out var hit, _chickenConfig.AreaOfSight))
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
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _chickenConfig.AreaOfSight);

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(_raycastOrigin, Vector3.down * _chickenConfig.RaycastDistance);
            Gizmos.DrawRay(_raycastOrigin, transform.right * _chickenConfig.RaycastDistance);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_config == null)
                Debug.LogError($"_config is null in GameObject {gameObject.name}");
            else if (!(_config as ChickenConfig))
                Debug.LogError($"_config is not ChickenConfig in GameObject {gameObject.name}");
        }
#endif
    }
}