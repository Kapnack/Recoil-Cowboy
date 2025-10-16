using Systems;
using UnityEngine;

public class ShaderSettings : MonoBehaviour
{
    IShaderSettings settings;

    private void Awake()
    {
        settings = ServiceProvider.GetService<IShaderSettings>();
    }

    public void ActiveShaderFlicker(bool active) => settings?.SetFlickerActive(active);
    
}