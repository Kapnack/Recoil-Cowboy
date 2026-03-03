using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button _creditsButton;
    private TMP_Text _text;
    private Color originalTextColor;

    [SerializeField] private Color buttonHoverColor = new Color(0.65f, 0.65f, 0.65f, 1);
    [SerializeField] private Color textHoverColor = new Color(0.7f, 0.7f, 0.7f, 1);

    private void Awake()
    {
        _creditsButton = gameObject.GetComponent<Button>();

        foreach (Transform child in gameObject.transform)
        {
            if (child.TryGetComponent<TMP_Text>(out _text))
                break;
        }

        if (_text != null)
            originalTextColor = _text.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _creditsButton.image.color = buttonHoverColor;

        if (_text != null)
            _text.color = textHoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _creditsButton.image.color = Color.white;

        if (_text != null)
            _text.color = originalTextColor;
    }
}