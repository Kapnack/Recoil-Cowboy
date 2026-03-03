using UnityEngine;
using UnityEngine.InputSystem;

public class CrossHair : MonoBehaviour
{
    private Mouse _mouse;

    private void Awake()
    {
        _mouse = Mouse.current;
    }

    private void Update()
    {
        transform.position = _mouse.position.ReadValue();
    }
}