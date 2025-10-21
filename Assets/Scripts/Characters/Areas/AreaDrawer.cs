using System;
using UnityEngine;

namespace Characters.Areas
{
    [Serializable]
    [RequireComponent(typeof(LineRenderer))]
    public abstract class AreaDrawer : MonoBehaviour
    {
        [Tooltip("Number of segments (higher = smoother arc)")] [SerializeField]
        protected int resolution = 30;

        protected LineRenderer LineRenderer;

        protected void Awake()
        {
            LineRenderer = GetComponent<LineRenderer>();
            LineRenderer.useWorldSpace = true;
            LineRenderer.loop = true;
            LineRenderer.startWidth = 0.1f;
            LineRenderer.endWidth = 0.1f;
            LineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        private void LateUpdate() => SetArea();
        
        protected abstract void SetArea();
    }
}