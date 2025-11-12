using UnityEngine;

namespace ScriptableObjects
{
   [CreateAssetMenu(fileName = "VolumeConfig", menuName = "ScriptableObjects/VolumeConfig")]
   public class VolumeSettings : ScriptableObject
   {
      [field: SerializeField] public int MasterVol {get ; set;}
      [field: SerializeField] public int SFXVol {get ; set;}
      [field: SerializeField] public int MusicVol {get ; set;}
   }
}
