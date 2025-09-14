using UnityEngine;

namespace MouseTracker
{
    public interface IMousePositionTracker
    {
        public Vector3 GetMouseDir(Transform transform);
        public Vector2 GetMouseScreenPos();
    }
}