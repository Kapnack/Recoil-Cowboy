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
            
            LineRenderer.alignment = LineAlignment.TransformZ;
            LineRenderer.generateLightingData = false;
            LineRenderer.textureMode = LineTextureMode.Stretch;
            
            var material = new Material(Shader.Find("Sprites/Default"));
            material.SetFloat("_Surface", 1f);
            material.SetOverrideTag("RenderType", "Transparent");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            material.SetFloat("_Blend", 0f);
            
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);

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