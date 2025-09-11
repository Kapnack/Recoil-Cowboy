using UnityEngine;

namespace MouseTracker
{
    public interface IMousePositionTracker
    {
        public Vector3 GetMouseDir();
        public Vector2 GetMouseScreenPos();
        public Vector3 GetMouseWorldPos();
    }
}