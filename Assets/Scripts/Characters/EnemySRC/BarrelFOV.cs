using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

namespace Characters.EnemySRC
{
    [RequireComponent(typeof(LineRenderer))]
    public class BarrelFOV : MonoBehaviour
    {
        [SerializeField] private BarrelEnemyConfig config;

        private readonly List<Vector3> _points = new();
   
        [Tooltip("Number of segments (higher = smoother arc)")]
        public int resolution = 30;

        private LineRenderer _lineRenderer;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.loop = false;
            _lineRenderer.startWidth = 0.1f;
            _lineRenderer.endWidth = 0.1f;
        }

        private void LateUpdate()
        {
            DrawVerticalCone();
        }

        private void DrawVerticalCone()
        {
            _points.Clear();
        
            var origin = transform.position + transform.right * config.RaycastOffSet;

            for (int i = 0; i <= resolution; i++)
            {
                var angle = Mathf.Lerp(-config.AttackRadius, config.AttackRadius, i / (float)resolution);
            
                var dir = Quaternion.Euler(0, 0, angle) * transform.right;
            
                var point = origin + dir * config.AttackDistance;

                _points.Add(point);
            }
        
            _points.Insert(0, origin);
            _points.Add(origin);

            _lineRenderer.positionCount = _points.Count;
            _lineRenderer.SetPositions(_points.ToArray());
        }
    }
}
