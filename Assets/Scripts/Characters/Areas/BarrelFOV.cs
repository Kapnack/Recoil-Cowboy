using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

namespace Characters.Areas
{
    [RequireComponent(typeof(LineRenderer))]
    public class BarrelFOV : AreaDrawer
    {
        [SerializeField] private BarrelEnemyConfig config;

        private readonly List<Vector3> _points = new();

        protected override void SetArea()
        {
            _points.Clear();
        
            var origin = transform.position + transform.right * config.RaycastOffSet;
            _points.Add(origin);

            for (int i = 0; i <= Resolution; i++)
            {
                var angle = Mathf.Lerp(-config.AttackRadius, config.AttackRadius, i / (float)Resolution);
            
                var dir = Quaternion.Euler(0, 0, angle) * transform.right;
            
                var point = origin + dir * config.AttackDistance;

                _points.Add(point);
            }
            
            LineRenderer.positionCount = _points.Count;
            LineRenderer.SetPositions(_points.ToArray());
        }
    }
}
