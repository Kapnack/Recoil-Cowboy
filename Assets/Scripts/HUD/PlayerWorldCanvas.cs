using System;
using System.Collections;
using Characters.PlayerSRC;
using Systems;
using Systems.CentralizeEventSystem;
using TMPro;
using UnityEngine;

public class PlayerWorldCanvas : MonoBehaviour
{
    private CanvasGroup _elements;
    private TMP_Text _text;

    private Coroutine _timer;

    private int _calls;

    private float _startTime = 0;

    private const int Speed = 2;

    private void Awake()
    {
        _elements = GetComponentInChildren<CanvasGroup>();
        _text = GetComponentInChildren<TMP_Text>();
        
        _elements.alpha = 0;
    }

    private void Start()
    {
        ServiceProvider.GetService<CentralizeEventSystem>().AddListener<AmmoChange>(OnReload);
    }

    private void OnDestroy()
    {
        ServiceProvider.GetService<CentralizeEventSystem>().RemoveListener<AmmoChange>(OnReload);
    }

    private void OnReload(int previous, int current, int max)
    {
        if (current == previous)
            return;

        SetStartTime();
        SetValues(previous, current, max);
        StartCoroutine(StartAnimation());

        if (_timer == null)
            _timer = StartCoroutine(StartTimer());
    }

    private IEnumerator StartTimer()
    {
        while (Time.time - _startTime < 1.0f)
            yield return null;
        
        StartCoroutine(EndAnimation());
        _timer = null;
    }

    private void SetValues(int previous, int current, int max)
    {
        if (current < previous)
        {
            if (_calls > 0)
                _calls = 0;

            --_calls;
            _text.color = Color.red;
        }
        else
        {
            if (_calls < 0)
                _calls = 0;

            ++_calls;
            _text.color = Color.green;
        }

        string sign = _calls > 0 ? "+" : "";

        _text.text = _calls != 0 ? $"{sign}{_calls}" : $"{_calls}";
    }


    private IEnumerator StartAnimation()
    {
        while (_elements.alpha < 1)
        {
            _elements.alpha += Time.deltaTime * Speed;
            yield return null;
        }
    }

    private IEnumerator EndAnimation()
    {
        StopCoroutine(StartAnimation());

        _calls = 0;

        while (_elements.alpha > 0)
        {
            _elements.alpha -= Time.deltaTime * Speed;
            yield return null;
        }
    }

    private void SetStartTime()
    {
        _startTime = Time.time;
    }
}