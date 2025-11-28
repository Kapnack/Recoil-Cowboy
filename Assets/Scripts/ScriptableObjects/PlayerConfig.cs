using System;
using UnityEngine;

namespace ScriptableObjects
{
    [Serializable]
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "ScriptableObjects/PlayerConfig")]
    public class PlayerConfig : CharacterConfig
    {
        [field: SerializeField] public int MaxLives { get; set; }
        [field: SerializeField] public float InvincibleTime { get; set; }
        [field: SerializeField] public int MaxBullets { get; set; }
        [field: SerializeField] public float KnockBack { get; set; }
        [field: SerializeField] public int PointsPerKill { get; set; }
        [field: SerializeField] public bool InfinitLives { get; set; }
        [field: SerializeField] public bool InfinitAmmo { get; set; }
    }
}