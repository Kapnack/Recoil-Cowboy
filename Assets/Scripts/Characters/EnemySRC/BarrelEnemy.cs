using System;
using ScriptableObjects;
using Systems.LayerClassGenerator;
using Systems.TagClassGenerator;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Characters.EnemySRC
{
    public class BarrelEnemy : Enemy
    {
        [SerializeField] private BarrelEnemyConfig config;

        [SerializeField] private GameObject bulletPrefab;

        private BoxCollider _collider;

        private IAnimate _animate;

        private Vector3 _targetDir;

        private bool _hidden;

        private bool _canShoot;

        private float _coldDownTimer;

        private bool Hidden
        {
            get => _hidden;
            set
            {
                if (_hidden == value)
                    return;

                _hidden = value;

                _animate.ChangeAnimation(value);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _animate = GetComponentInChildren<IAnimate>();
            Rb.isKinematic = true;
            _collider = GetComponent<BoxCollider>();
        }

        public override void SetUp(Action action = null)
        {
            base.SetUp(action);

            if (Physics.BoxCast(
                    new Vector3(transform.position.x, transform.position.y + _collider.size.y * 0.5f,
                        0), _collider.size * 0.5f, Vector3.down, out RaycastHit hit,
                    Quaternion.identity, Mathf.Infinity, LayerMask.GetMask(Layers.Environment)))
            {
                transform.position = new Vector3(hit.point.x, hit.point.y + _collider.size.y * 0.5f, 0);
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
            Collider[] colliderHits = Physics.OverlapSphere(transform.position, config.AreaOfSight);

            foreach (Collider hit in colliderHits)
            {
                if (hit.gameObject == gameObject || !hit.CompareTag(Tags.Player))
                    continue;

                Vector3 dir = (hit.transform.position - transform.position).normalized;

                if (Physics.Raycast(transform.position, dir, out RaycastHit hitInfo, config.AreaOfSight))
                {
                    if (hitInfo.collider.CompareTag(Tags.Player))
                        return true;
                }
            }

            return false;
        }

        private void Shoot()
        {
            GameObject bulletGO = Instantiate(bulletPrefab, transform.position + transform.right * config.FireOffset,
                gameObject.transform.rotation);

            SceneManager.MoveGameObjectToScene(bulletGO, gameObject.scene);

            if (bulletGO.TryGetComponent(out Bullet bullet))
                bullet.Launch(this, transform.position + transform.right * config.FireOffset,
                    _targetDir, config.FireForce);

            _coldDownTimer = config.ColdDown + Time.time;
        }

        public override void ReceiveDamage(Action action = null)
        {
            if (!Hidden)
                base.ReceiveDamage(action);
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