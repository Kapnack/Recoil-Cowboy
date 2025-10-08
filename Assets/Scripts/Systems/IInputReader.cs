using UnityEngine.InputSystem;

namespace Systems
{
    public interface IInputReader
    {
        public void ActivePlayerMap();
        public void DeactivatePlayerMap();
        public void SwitchPlayerMapState();
    }
}