using UnityEngine;

[CreateAssetMenu(fileName = "PointsStats", menuName = "ScriptableObjects/PointsStats")]
public class PointsStats : ScriptableObject
{
    [field: SerializeField] public int CurrentKillPoints { get; set; }
    [field: SerializeField] public int RecordKillPoints { get; set; }
    [field: SerializeField] public int CurrentDistance { get; set; }
    [field: SerializeField] public int RecordDistance { get; set; }

    public bool NewDistanceRecord;
    public bool NewKillPointsRecord;
}
