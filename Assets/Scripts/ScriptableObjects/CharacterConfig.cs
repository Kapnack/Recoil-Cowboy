using System;
using UnityEngine;

namespace ScriptableObjects
{
    [Serializable]
    public class CharacterConfig : ScriptableObject
    {
        [field: SerializeField] public float AreaOfSight { get; set; }
    }
}