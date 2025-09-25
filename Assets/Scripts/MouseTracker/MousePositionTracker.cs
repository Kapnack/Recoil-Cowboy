using Systems;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MouseTracker
{
    public class MousePositionTracker : MonoBehaviour, IMousePositionTracker
    {
        private Camera _camera;
        private Mouse _mouse;

        private Vector2 _mouseScreenPos;
        private Vector3 _mouseWorldPos;
        private float _mouseDepth;
        private Vector3 _mouseDir;

        private void Awake()
        {
            ServiceProvider.SetService<IMousePositionTracker>(this);
        }

        private void Start()
        {
            _camera = Camera.main;
            _mouse = Mouse.current;
        }

        private void Update()
        {
            _mouseScreenPos = _mouse.position.ReadValue();
        }

        public Vector3 GetMouseDir(Transform other)
        {
            _mouseDepth = _camera.WorldToScreenPoint(other.transform.position).z;
            _mouseWorldPos = _camera.ScreenToWorldPoint(new Vector3(_mouseScreenPos.x, _mouseScreenPos.y, _mouseDepth));
            _mouseWorldPos.z = other.transform.position.z;

            _mouseDir = (_mouseWorldPos - other.position).normalized;
            _mouseDir.z = 0.0f;

            return _mouseDir;
        }

        public Vector2 GetMouseScreenPos() => _mouseScreenPos;
        public Vector3 GetMouseWorldPos() => _mouseWorldPos;
    }
}