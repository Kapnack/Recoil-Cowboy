using System.Collections;
using Systems;
using Systems.CentralizeEventSystem;
using Systems.SceneLoader;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    private ILoadingData _data;

    private CentralizeEventSystem _eventSystem;
    
    private bool _isLoading;

    [FormerlySerializedAs("_canvas")]
    [Header("Canvas")]
    [SerializeField]
    private Canvas canvas;

    [SerializeField] private Slider slider;

    private void Awake()
    {
        _eventSystem = ServiceProvider.GetService<CentralizeEventSystem>();
        
        slider.minValue = 0;
        slider.maxValue = 100;

        StartCoroutine(GetLoadingData());
    }

    private IEnumerator GetLoadingData()
    {
        while (!ServiceProvider.TryGetService(out _data))
            yield return null;
        
        _eventSystem.AddListener<LoadingStarted>(StartLoadingScreen);
        _eventSystem.AddListener<LoadingEnded>(EndLoadingScreen);
    }

    private void StartLoadingScreen()
    {
        canvas?.gameObject.SetActive(true);

        _isLoading = true;
        StartCoroutine(UpdateLoading());
    }

    private IEnumerator UpdateLoading()
    {
        float currentProgress = 0;
        float taskProgress = 0;

        while (_isLoading)
        {
            taskProgress = _data.GetCurrentLoadingProgress() != 0 ? _data.GetCurrentLoadingProgress() * 100.0f : 0;
            currentProgress = taskProgress < currentProgress ? currentProgress : taskProgress;

            slider.value = currentProgress;

            if (!Mathf.Approximately(currentProgress, 100.0f))
                yield return null;
        }
    }

    private void EndLoadingScreen()
    {
        _isLoading = false;
        canvas?.gameObject.SetActive(false);
    }
}