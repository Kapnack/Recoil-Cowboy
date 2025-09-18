using System.Collections.Generic;
using UnityEngine;

namespace Systems.Pool
{
    public class PooledObjectInfo
    {
        public string Name;
        public readonly List<GameObject> InactiveObjects = new();
    }
}