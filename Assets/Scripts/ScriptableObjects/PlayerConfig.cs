using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects
{
    [Serializable]
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "ScriptableObjects/PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        [field: SerializeField] public int MaxLives { get; set; }
        [field: SerializeField] public float InvincibleTime { get; set; }
        [field: SerializeField] public int MaxBullets { get; set; }
        [field: SerializeField] public float MaxDistance { get; set; }
        [field: SerializeField] public float KnockBack { get; set; }
    }
}