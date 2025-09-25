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

    private bool _isLoading;

    [FormerlySerializedAs("_canvas")] [Header("Canvas")] [SerializeField]
    private Canvas canvas;

    [SerializeField] private Slider slider;

    private void Awake()
    {
        slider.minValue = 0;
        slider.maxValue = 100;

        StartCoroutine(GetLoadingData());
    }

    private IEnumerator GetLoadingData()
    {
        while (!ServiceProvider.TryGetService(out _data))
            yield return null;

        ICentralizeEventSystem eventSystem;
        while (!ServiceProvider.TryGetService(out eventSystem))
            yield return null;

        SimpleEvent simpleEvent;
        while (!eventSystem.TryGet(GameManagerKeys.LoadingStarted, out simpleEvent))
            yield return null;
        simpleEvent.AddListener(StartLoadingScreen);

        while (!eventSystem.TryGet(GameManagerKeys.LoadingEnded, out simpleEvent))
            yield return null;
        simpleEvent.AddListener(EndLoadingScreen);
    }

    private void StartLoadingScreen()
    {
        canvas?.gameObject.SetActive(true);

        _isLoading = true;
        StartCoroutine(UpdateLoading());
    }

    private IEnumerator UpdateLoading()
    {
        while (_isLoading)
        {
            var progress = _data.GetCurrentLoadingProgress() != 0 ? _data.GetCurrentLoadingProgress() * 100.0f : 0;

            slider.value = progress;

            yield return null;
        }
    }

    private void EndLoadingScreen()
    {
        _isLoading = false;
        canvas?.gameObject.SetActive(false);
    }
}