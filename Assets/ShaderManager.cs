using System.Collections;
using Systems;
using UnityEngine;

public class ShaderManager : MonoBehaviour, IShaderManager
{
    [SerializeField] private Material material;

    private const string Variable = "_Intensity";
    
    private void Awake()
    {
        ServiceProvider.SetService<IShaderManager>(this);
        
        material.SetFloat(Variable, 0);
    }

    private IEnumerator OnTransition()
    {
        var intensity = material.GetFloat(Variable);

        while (intensity < 1)
        {
            intensity += Time.deltaTime;
            material.SetFloat(Variable, intensity);
            yield return null;
        }
        
        intensity = Mathf.Clamp(intensity, 0, 1);
    }

    private IEnumerator OffTransition()
    {
        var intensity = material.GetFloat(Variable);

        while (intensity > 0)
        {
            intensity -= Time.deltaTime;
            material.SetFloat(Variable, intensity);
            yield return null;
        }
        intensity = Mathf.Clamp(intensity, 0, 1);
    }
    
    public void StartOnTransition() => StartCoroutine(OnTransition());

    public void StartOffTransition() => StartCoroutine(OffTransition());
}