using MouseTracker;
using ScriptableObjects;
using Systems;
using UnityEngine;
using UnityEngine.UI;

public class KnockbackMeter : MonoBehaviour
{
    [SerializeField] private Transform source;

    private Image _sightImage;
    private IMousePositionTracker _mouseTracker;

    [Header("Settings")] [SerializeField] private PlayerConfig playerConfig;
    
    private Vector3 _distance;
    private Color _originalColor;
    [SerializeField] private Color secondaryColor;
    private float _mapped;

    private void Awake()
    {
        _sightImage = GetComponent<Image>();

        _mouseTracker = ServiceProvider.GetService<IMousePositionTracker>();

        if (!source || _sightImage == null || _mouseTracker == null)
            enabled = false;

        _originalColor = _sightImage.color;
    }

    private void Update()
    {
        _distance = source.position - _mouseTracker.GetMouseWorldPos();

        _mapped = Mathf.InverseLerp(0.0f, playerConfig.AreaOfSight * playerConfig.AreaOfSight, _distance.sqrMagnitude);

        _sightImage.color = Color.Lerp(_originalColor, secondaryColor, _mapped);
    }
}