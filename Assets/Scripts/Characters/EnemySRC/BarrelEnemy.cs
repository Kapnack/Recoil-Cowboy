using System;
using System.Collections;
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

        private Coroutine _corrutine;

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

            _corrutine = StartCoroutine(AdjustPosition());
        }

        private IEnumerator AdjustPosition()
        {
            yield return new WaitForSeconds(0.1f);

            Vector3 worldSize = Vector3.Scale(_collider.size, transform.lossyScale);
            Vector3 origin = transform.position;

            float duration = 360;

#if UNITY_EDITOR
            Debug.DrawRay(new Vector3(origin.x + worldSize.x * 0.5f, origin.y, origin.z - worldSize.z * 0.5f),
                Vector3.down * 100, Color.magenta, duration);
            Debug.DrawRay(new Vector3(origin.x - worldSize.x * 0.5f, origin.y, origin.z + worldSize.z * 0.5f),
                Vector3.down * 100, Color.magenta, duration);
            Debug.DrawRay(origin, Vector3.down * 100, Color.magenta, duration);
            Debug.DrawRay(new Vector3(origin.x + worldSize.x * 0.5f, origin.y, origin.z + worldSize.z * 0.5f),
                Vector3.down * 100, Color.magenta, duration);
            Debug.DrawRay(new Vector3(origin.x - worldSize.z * 0.5f, origin.y, origin.z - worldSize.z * 0.5f),
                Vector3.down * 100, Color.magenta, duration);
#endif
            if (Physics.BoxCast(origin, worldSize * 0.5f, Vector3.down, out RaycastHit hit,
                    transform.rotation, Mathf.Infinity, LayerMask.GetMask(Layers.Environment),
                    QueryTriggerInteraction.Ignore))
            {
                Debug.Log($"Raycast hit  {hit.collider.name} at {hit.point.y}", hit.collider.gameObject);
                transform.position = new Vector3(transform.position.x, hit.point.y + worldSize.y * 0.5f,
                    transform.position.z);
            }
            else
            {
                Debug.Log($"{nameof(gameObject)} did not detect any platform");
            }

            _corrutine = null;
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

            bulletGO.transform.up = _targetDir;

            if (bulletGO.TryGetComponent(out Bullet bullet))
                bullet.Launch(this, transform.position + transform.right * config.FireOffset,
                    _targetDir, config.FireForce);

            _coldDownTimer = config.ColdDown + Time.time;
        }

        public override void ReceiveDamage(Action action = null)
        {
            if (Hidden) 
                return;
            
            if (_corrutine != null)
                StopCoroutine(_corrutine);

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