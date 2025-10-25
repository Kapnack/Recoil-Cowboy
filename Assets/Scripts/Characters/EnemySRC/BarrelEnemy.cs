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
            Rb.isKinematic = true;
        }

        private void OnEnable() => SetUp();
        
        protected override void SetUp()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity))
            {
                transform.position = hit.collider.gameObject.transform.position + transform.localScale;
            }
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
            Vector3 origin = transform.position + transform.right * config.RaycastOffSet;

            Collider[] raycastHits = Physics.OverlapSphere(origin, config.AttackDistance);

            foreach (Collider t in raycastHits)
            {
                if (!t.CompareTag(Tags.Player))
                    continue;

                Transform target = t.transform;
                _targetDir = (target.position - origin).normalized;

                float angle = Vector3.Angle(transform.right, _targetDir);

                if (angle > -config.AttackRadius && angle < config.AttackRadius)
                {
                    float targetDistance = Vector3.Distance(origin, target.position);

                    if (!Physics.Raycast(origin, _targetDir, out RaycastHit hit, targetDistance))
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
                if (hit.gameObject == gameObject || !hit.CompareTag(Tags.Player))
                    continue;

                var dir = (hit.transform.position - transform.position).normalized;
                
                if (Physics.Raycast(transform.position, dir, out var hitInfo, config.AreaOfSight))
                {
                    if (hitInfo.collider.CompareTag(Tags.Player))
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