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

    [Header("Image")]
    [SerializeField] private Image fillArea;
    
    [Header("Settings")] [SerializeField] private PlayerConfig playerConfig;

    [Header("Options")] [SerializeField] [Range(0f, 100f)]
    private float smoothSpeed = 10f;

    private float _currentFill;
    private Vector3 _distance;
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
        _distance = source.position - endPoint.position;

        _mapped = Mathf.InverseLerp(startPoint.position.y, endPoint.position.y, _distance.y);
        
        _currentFill = smoothSpeed > 0f ? Mathf.Lerp(_currentFill, _mapped, Time.deltaTime * smoothSpeed) : _mapped;
        
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