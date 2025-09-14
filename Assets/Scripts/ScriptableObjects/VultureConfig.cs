using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "VultureConfig", menuName = "ScriptableObjects/VultureConfig")]
    public class VultureConfig : ScriptableObject
    {
        [field: SerializeField] public float MaxVelocity { get; private set; }
        [field: SerializeField] public float MoveSpeed { get; private set; }
        
        [field: SerializeField] public float  AreaOfSight { get; private set; }
    }
}