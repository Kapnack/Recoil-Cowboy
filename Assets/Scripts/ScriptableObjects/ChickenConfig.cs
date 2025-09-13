using System;
using UnityEngine;

namespace ScriptableObjects
{
    [Serializable]
    [CreateAssetMenu(fileName = "ChickenConfig", menuName = "ScriptableObjects/ChickenConfig")]
    public class ChickenConfig : EnemyConfig
    {
       [field: SerializeField] public float RaycastOffSet {get; private set;}
       [field: SerializeField] public float RaycastDistance {get; private set;}
    }
}