using System;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

namespace Characters.Areas
{
    [Serializable] [RequireComponent(typeof(LineRenderer))]
    public class CharacterArea : AreaDrawer
    {
        [SerializeField] private CharacterConfig config;
        
        protected override void SetArea()
        {
            var points = new List<Vector3>();

            LineRenderer.positionCount = resolution;

            for (int i = 0; i < resolution; i++)
            {
                var currentRadian = (float)i / resolution * 2 * Mathf.PI;

                var xScaled = Mathf.Cos(currentRadian);
                var yScaled = Mathf.Sin(currentRadian);

                var x = xScaled * config.AreaOfSight;
                var y = yScaled * config.AreaOfSight;

                var currentPos = transform.position + new Vector3(x, y, 0);

                points.Add(currentPos);
            }
            
            LineRenderer.SetPositions(points.ToArray());
        }
    }
}