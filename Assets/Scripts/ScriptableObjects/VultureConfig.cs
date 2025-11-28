using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "VultureConfig", menuName = "ScriptableObjects/VultureConfig")]
    public class VultureConfig : CharacterConfig
    {
        [field: SerializeField] public float MaxVelocity { get; private set; }
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [field: SerializeField] public float SmoothBackTime { get; private set; }
    }
}