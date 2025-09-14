using ScriptableObjects;
using Systems.TagClassGenerator;
using UnityEngine;
using UnityEngine.Serialization;

namespace Characters.EnemySRC
{
    public class BarrelEnemy : Enemy
    {
        private BarrelEnemyConfig _barrelConfig;

        [SerializeField] private GameObject bulletPrefab;

        public bool Hidden { get; private set; }

        private GameObject bullets;
        private float _coldDownTimer;

        private void Awake()
        {
            _barrelConfig = (BarrelEnemyConfig)_config;
        }

        private void FixedUpdate()
        {
            Hidden = ShouldHide();

            if (Hidden)
                return;
            
            if (_coldDownTimer > Time.time)
                return;

            if (TargetInSight())
                Shoot();
        }

        private bool TargetInSight()
        {
            var raycastHits = Physics.SphereCastAll(transform.position + transform.right * _barrelConfig.FireOffset,
                _barrelConfig.AttackRadius,
                transform.right, _barrelConfig.AttackRadius);

            foreach (var obj in raycastHits)
            {
                if (!obj.collider.CompareTag(Tags.Player))
                    continue;

                var dir = (obj.transform.position - transform.position).normalized;

                if (Physics.Raycast(transform.position + transform.right * _barrelConfig.FireOffset, dir,
                        _barrelConfig.RaycastDistance))
                {
                    return true;
                }
            }

            return false;
        }

        private bool ShouldHide()
        {
            var colliderHits = Physics.OverlapSphere(transform.position, _barrelConfig.AreaOfSight);

            Hidden = false;

            foreach (var hit in colliderHits)
            {
                if (hit.gameObject == this.gameObject)
                    continue;

                if (hit.TryGetComponent<IHealthSystem>(out var healthSystem))
                {
                    return true;
                }
            }

            return false;
        }

        private void Shoot()
        {
            var bulletGO = Instantiate(bulletPrefab, transform.position + transform.right * _barrelConfig.FireOffset,
                gameObject.transform.rotation);

            if (bulletGO.TryGetComponent<Bullet>(out var bullet))
                bullet.Launch(this, transform.position + transform.right * _barrelConfig.FireOffset,
                    _barrelConfig.FireForce);

            _coldDownTimer = _barrelConfig.ColdDown + Time.time;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _barrelConfig.AreaOfSight);

            if (Hidden)
                return;

            Gizmos.color = Color.cyan;

            var origin = transform.position + transform.right * _barrelConfig.FireOffset;
            var direction = transform.right.normalized;

            // Start sphere
            Gizmos.DrawWireSphere(origin, _barrelConfig.AttackRadius);

            // End sphere
            var end = origin + direction * _barrelConfig.RaycastDistance;
            Gizmos.DrawWireSphere(end, _barrelConfig.AttackRadius);

            // Direction line
            Gizmos.DrawLine(origin, end);
        }

        public override void ReceiveDamage()
        {
            if (!Hidden)
                base.ReceiveDamage();
        }
    }
}