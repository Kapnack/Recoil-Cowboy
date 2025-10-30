using System.Collections;
using MouseTracker;
using Systems;
using Systems.CentralizeEventSystem;
using UnityEngine;

namespace Characters.PlayerSRC
{
    public class SpinReload : MonoBehaviour
    {
        private Transform _target;

        [SerializeField] private float spinTime = 1f;
        private float _accumulatedDeg;
        private float _timer;
        private float _prevAngle;
        private float _depth;
        private bool _hasPrev;

        private Vector2 _mousePos;
        private Vector3 _mouseDir;
        private Vector3 _mouseWorld;
        private float _angle;
        private float _delta;

        private IMousePositionTracker _mouseTracker;

        private readonly SimpleEvent _reloadEvent = new();

        private void Awake()
        {
            _target = gameObject.transform;

            ServiceProvider.TryGetService(out ICentralizeEventSystem eventSystem);

            eventSystem.Register(PlayerEventKeys.Reload, _reloadEvent);

            ServiceProvider.TryGetService(out _mouseTracker);
        }

        private void OnDestroy()
        {
            ServiceProvider.GetService<ICentralizeEventSystem>().Unregister(PlayerEventKeys.Reload);
        }

        private void Update()
        {
            _mouseDir = _mouseTracker.GetMouseDir(_target);

            _angle = Mathf.Atan2(_mouseDir.y, _mouseDir.x) * Mathf.Rad2Deg;

            if (!_hasPrev)
            {
                _prevAngle = _angle;
                _hasPrev = true;
                return;
            }


            float delta = Mathf.DeltaAngle(_prevAngle, _angle);
            _prevAngle = _angle;

            _accumulatedDeg += delta;
            _timer += Time.deltaTime;

            if (Mathf.Abs(_accumulatedDeg) >= 360f && _timer <= spinTime)
            {
                _reloadEvent?.Invoke();

                _accumulatedDeg = 0f;
                _timer = 0f;
                _hasPrev = false;
                return;
            }

            if (_timer > spinTime)
            {
                _accumulatedDeg = 0f;
                _timer = 0f;
            }
        }
    }
}