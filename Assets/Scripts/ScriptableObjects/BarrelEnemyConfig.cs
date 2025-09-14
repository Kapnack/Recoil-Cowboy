using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects
{
    public class BarrelEnemyConfig : ChickenConfig
    {
        [field: SerializeField] public float AttackRadius { get; private set; }
        [field: SerializeField] public float FireOffset { get; private set; }
        [field: SerializeField] public float FireForce { get; private set; }
        [field: SerializeField] public float ColdDown { get; private set; }
    }
}