using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "BarrelEnemyConfig", menuName = "ScriptableObjects/BarrelEnemyConfig")]
    public class BarrelEnemyConfig : ScriptableObject
    {
        [field: SerializeField] public float AreaOfSight  { get; private set; }
       [field: SerializeField] public float AttackRadius { get; private set; }
       [field: SerializeField] public float FireOffset { get; private set; }
       [field: SerializeField] public float FireForce { get; private set; }
       [field: SerializeField] public float ColdDown { get; private set; }
       [field: SerializeField] public float RaycastOffSet { get; private set; }
       [field: SerializeField] public float RaycastDistance { get; private set; }
    }
}