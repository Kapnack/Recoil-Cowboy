using System;
using UnityEngine;

namespace Chunks
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private SpawnerType spawnerType;
        [SerializeField] private LayerMask layerMask;
        private static bool _initialized;

        private void Awake()
        {
            if (spawnerType != SpawnerType.Ground)
                return;

            float rightFreeSpace = 0;
            float leftFreeSpace = 0;

            if (Physics.Raycast(transform.position, Vector3.right, out RaycastHit hit, Mathf.Infinity, layerMask))
                rightFreeSpace = hit.distance;

            if (Physics.Raycast(transform.position, Vector3.left, out hit, Mathf.Infinity, layerMask))
                leftFreeSpace = hit.distance;

            transform.rotation = Quaternion.Euler(0.0f, leftFreeSpace > rightFreeSpace ? 180.0f : 0.0f, 0.0f);
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.5f);
        }
#endif
    }

    [Serializable]
    public enum SpawnerType
    {
        [InspectorName("Ground")] Ground,
        [InspectorName("Air")] Air
    }
}