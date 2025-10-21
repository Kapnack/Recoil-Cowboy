using System;
using ScriptableObjects;
using Systems.TagClassGenerator;
using UnityEngine;

namespace Characters.EnemySRC
{
    public class BarrelEnemy : Enemy
    {
        [SerializeField] private BarrelEnemyConfig config;

        [SerializeField] private GameObject bulletPrefab;

        private Vector3 _targetDir;

        private bool Hidden { get; set; }

        private float _coldDownTimer;

        protected override void Awake()
        {
            base.Awake();
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
            var origin = transform.position + transform.right * config.RaycastOffSet;

            var raycastHits = Physics.OverlapSphere(origin, config.AttackDistance);

            foreach (var t in raycastHits)
            {
                if (!t.CompareTag(Tags.Player))
                    continue;

                var target = t.transform;
                _targetDir = (target.position - origin).normalized;

                var angle = Vector3.Angle(transform.right, _targetDir);

                if (angle > -config.AttackRadius && angle < config.AttackRadius)
                {
                    var targetDistance = Vector3.Distance(origin, target.position);

                    if (!Physics.Raycast(origin, _targetDir, out var hit, targetDistance))
                        return false;

                    if (hit.collider.CompareTag(Tags.Player))
                        return true;
                }
            }

            return false;
        }


        private bool ShouldHide()
        {
            var colliderHits = Physics.OverlapSphere(transform.position, config.AreaOfSight);

            foreach (var hit in colliderHits)
            {
                if (hit.gameObject == gameObject)
                    continue;

                if (hit.TryGetComponent<IHealthSystem>(out _))
                {
                    return true;
                }
            }

            return false;
        }

        private void Shoot()
        {
            var bulletGO = Instantiate(bulletPrefab, transform.position + transform.right * config.FireOffset,
                gameObject.transform.rotation);

            if (bulletGO.TryGetComponent<Bullet>(out var bullet))
                bullet.Launch(this, transform.position + transform.right * config.FireOffset,
                    _targetDir, config.FireForce);

            _coldDownTimer = config.ColdDown + Time.time;
        }

        public override void ReceiveDamage()
        {
            if (!Hidden)
                base.ReceiveDamage();
        }

        private void OnDrawGizmos()
        {
            if (!config)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, config.AreaOfSight);
        }
    }
}