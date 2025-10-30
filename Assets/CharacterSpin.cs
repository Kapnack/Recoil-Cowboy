using System.Collections;
using Systems;
using Systems.CentralizeEventSystem;
using Systems.LayerClassGenerator;
using UnityEngine;

public class CharacterSpin : MonoBehaviour
{ 
    private bool _spin = false;
    private float spinAmount = 0;
    private float _startingRotation = 0;
    private float _waitToCheck = 0;
    private void Start()
    {
        _startingRotation = transform.localRotation.z;
        
        ICentralizeEventSystem eventSystem = ServiceProvider.GetService<ICentralizeEventSystem>();
        
        eventSystem.Get(PlayerEventKeys.Attack).AddListener(StartSpin);
    }

    private void StartSpin()
    {
        _spin = true;
        _waitToCheck = 0.2f;
    }

    private void StopSpin() => _spin = false;
    
    
    private void Update()
    {
        if (!_spin)
            return;
        
        transform.localRotation = 
            Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, transform.localRotation.z + spinAmount);
            
        ++spinAmount;
        --_waitToCheck;
        
        Debug.DrawRay(gameObject.transform.position, Vector3.down, Color.red, 1);
        
        
        if(_waitToCheck < 0)
        if (Physics.Raycast(gameObject.transform.position, Vector3.down, out RaycastHit hit, 1f, LayerMask.GetMask(Layers.Environment)))
        {
            transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, _startingRotation);
            StopSpin();
        }
    } 
}
