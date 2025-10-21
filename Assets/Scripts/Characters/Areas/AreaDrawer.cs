using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Characters.Areas
{
    [Serializable]
    [RequireComponent(typeof(LineRenderer))]
    public abstract class AreaDrawer : MonoBehaviour
    {
        [Tooltip("Number of segments (higher = smoother arc)")]
        protected const int Resolution = 60;

        [FormerlySerializedAs("color")] [SerializeField] protected Color desireColor;

        protected LineRenderer LineRenderer;

        protected void Awake()
        {
            LineRenderer = GetComponent<LineRenderer>();
            LineRenderer.useWorldSpace = true;
            LineRenderer.loop = true;
            LineRenderer.startWidth = 0.5f;
            LineRenderer.endWidth = 0.5f;
            LineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            
            LineRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            LineRenderer.material.SetColor("_BaseColor", desireColor);
            
            LineRenderer.startColor = desireColor;
            LineRenderer.endColor = desireColor;
        }

        private void LateUpdate() => SetArea();
        
        protected abstract void SetArea();
    }
}