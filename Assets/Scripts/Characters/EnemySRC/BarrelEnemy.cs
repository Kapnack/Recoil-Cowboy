using ScriptableObjects;
using Systems.TagClassGenerator;
using UnityEngine;

namespace Characters.EnemySRC
{
    public class BarrelEnemy : Enemy
    {
        [SerializeField] private BarrelEnemyConfig barrelConfig;

        [SerializeField] private GameObject bulletPrefab;

        private bool Hidden { get; set; }
        
        private float _coldDownTimer;

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
            var raycastHits = Physics.SphereCastAll(transform.position + transform.right * barrelConfig.FireOffset,
                barrelConfig.AttackRadius,
                transform.right, barrelConfig.RaycastDistance);

            foreach (var obj in raycastHits)
            {
                if (!obj.collider.CompareTag(Tags.Player))
                    continue;

                var dir = (obj.transform.position - transform.position).normalized;

                if (Physics.Raycast(transform.position + transform.right * barrelConfig.FireOffset, dir,
                        barrelConfig.RaycastDistance))
                {
                    return true;
                }
            }

            return false;
        }

        private bool ShouldHide()
        {
            var colliderHits = Physics.OverlapSphere(transform.position, barrelConfig.AreaOfSight);

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
            var bulletGO = Instantiate(bulletPrefab, transform.position + transform.right * barrelConfig.FireOffset,
                gameObject.transform.rotation);

            if (bulletGO.TryGetComponent<Bullet>(out var bullet))
                bullet.Launch(this, transform.position + transform.right * barrelConfig.FireOffset,
                    barrelConfig.FireForce);

            _coldDownTimer = barrelConfig.ColdDown + Time.time;
        }

        public override void ReceiveDamage()
        {
            if (!Hidden)
                base.ReceiveDamage();
        }
        
        private void OnDrawGizmos()
        {
            if (!barrelConfig)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, barrelConfig.AreaOfSight);

            if (Hidden)
                return;

            Gizmos.color = Color.cyan;

            var origin = transform.position + transform.right * barrelConfig.FireOffset;
            var direction = transform.right.normalized;

            // Start sphere
            Gizmos.DrawWireSphere(origin, barrelConfig.AttackRadius);

            // End sphere
            var end = origin + direction * barrelConfig.RaycastDistance;
            Gizmos.DrawWireSphere(end, barrelConfig.AttackRadius);

            // Direction line
            Gizmos.DrawLine(origin, end);
        }
    }
}