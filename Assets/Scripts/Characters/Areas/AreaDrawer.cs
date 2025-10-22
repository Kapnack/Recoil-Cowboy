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
            LineRenderer.startWidth = 0.1f;
            LineRenderer.endWidth = 0.1f;
            LineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            
            var material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            material.SetFloat("_Surface", 1f);
            material.SetFloat("_Blend", 0f);
            material.SetOverrideTag("RenderType", "Transparent");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            
            var transparentColor = desireColor;
            transparentColor.a = 0.1f;

            material.SetColor("_BaseColor", transparentColor);

            LineRenderer.material = material;
            LineRenderer.startColor = transparentColor;
            LineRenderer.endColor = transparentColor;
        }


        private void LateUpdate() => SetArea();
        
        protected abstract void SetArea();
    }
}