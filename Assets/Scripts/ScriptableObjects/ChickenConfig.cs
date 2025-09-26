using System;
using UnityEngine;

namespace ScriptableObjects
{
    [Serializable]
    [CreateAssetMenu(fileName = "ChickenConfig", menuName = "ScriptableObjects/ChickenConfig")]
    public class ChickenConfig : ScriptableObject
    {
        [field: SerializeField] public float MaxVelocity { get; private set; }
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [field: SerializeField] public float JumpForce { get; private set; }
        [field: SerializeField] public float JumpMinTimer { get; private set; }
        [field: SerializeField] public float JumpMaxTimer { get; private set; }
        [field: SerializeField] public float AreaOfSight { get; private set; }
       [field: SerializeField] public float RaycastOffSet {get; private set;}
       [field: SerializeField] public float RaycastDistance {get; private set;}
    }
}