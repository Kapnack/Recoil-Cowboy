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
            _camera = Camera.main;
            _mouse = Mouse.current;
        
            ServiceProvider.SetService<IMousePositionTracker>(this);
        }

        private void Update()
        {
            _mouseScreenPos = _mouse.position.ReadValue();

            _mouseDepth = _camera.WorldToScreenPoint(transform.position).z;
            _mouseWorldPos = _camera.ScreenToWorldPoint(new Vector3(_mouseScreenPos.x, _mouseScreenPos.y, _mouseDepth));
            _mouseWorldPos.z = transform.position.z;
        }

        public Vector3 GetMouseDir()
        {
            _mouseDir = (_mouseWorldPos - transform.position).normalized;
            _mouseDir.z = 0.0f;

            return _mouseDir;
        }

        public Vector2 GetMouseScreenPos() => _mouseScreenPos;

        public Vector3 GetMouseWorldPos() => _mouseWorldPos;
    }
}