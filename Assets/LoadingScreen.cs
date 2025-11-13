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

    [SerializeField] private int _loadingWait = 2;

    [FormerlySerializedAs("_canvas")]
    [Header("Canvas")]
    [SerializeField]
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
        StartCoroutine(CloseLoadingScreen());
    }

    private IEnumerator CloseLoadingScreen()
    {
        yield return new WaitForSeconds(_loadingWait);

        _isLoading = false;
        canvas?.gameObject.SetActive(false);
    }
}