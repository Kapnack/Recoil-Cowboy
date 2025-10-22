using MouseTracker;
using ScriptableObjects;
using Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressMeter : MonoBehaviour
{
    private Slider _slider;

    [SerializeField] private TMP_Text percentText;

    private string _textFormat;

    [SerializeField] private Transform source;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    [Header("Image")] [SerializeField] private Image fillArea;

    [Header("Options")] [SerializeField] private bool horizontalLevel;
    [SerializeField] [Range(0f, 20f)] private float smoothSpeed = 10f;

    private float _currentFill;
    private Color _targetColor;
    private float _mapped;

    private void Awake()
    {
        if (percentText)
        {
            _textFormat = percentText.text;
            percentText.text = string.Format(_textFormat, 0.0f);
        }

        _slider = GetComponent<Slider>();

        if (!_slider || !source || fillArea == null || !startPoint || !endPoint)
        {
            enabled = false;
            return;
        }

        _slider.minValue = 0;
        _slider.maxValue = 1;
    }

    private void Update()
    {
        if (horizontalLevel)
            _mapped = Mathf.InverseLerp(startPoint.position.x, endPoint.position.x, source.position.x);
        else
            _mapped = Mathf.InverseLerp(startPoint.position.y, endPoint.position.y, source.position.y);

        _currentFill = smoothSpeed > 0f
            ? Mathf.MoveTowards(_currentFill, _mapped, Time.deltaTime * smoothSpeed)
            : _mapped;

        _slider.value = Mathf.Clamp01(_currentFill);

        percentText.text = string.Format(_textFormat, (int)(_currentFill * 100.0f));

        _targetColor = _currentFill switch
        {
            > 0.75f => Color.green,
            > 0.50f => Color.yellow,
            > 0.25f => new Color(1f, 0.5f, 0f),
            < 0.01f => new Color(0.0f, 0.0f, 0.0f, 0.0f),
            < 0.25f => Color.red,
            _ => Color.white
        };

        fillArea.color = Color.Lerp(fillArea.color, _targetColor, Time.deltaTime * 5f);
    }
}