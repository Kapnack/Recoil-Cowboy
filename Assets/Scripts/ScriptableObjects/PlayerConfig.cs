using System;
using UnityEngine;

namespace ScriptableObjects
{
    [Serializable]
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "ScriptableObjects/PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        [field: SerializeField] public int MaxLives { get; private set; }
        [field: SerializeField] public float InvincibleTime { get; private set; }
        [field: SerializeField] public int MaxBullets { get; private set; }
        [field: SerializeField] public float KnockBack { get; private set; }
    }
}