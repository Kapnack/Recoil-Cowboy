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
        _text = gameObject.GetComponentInChildren<TMP_Text>();
        originalTextColor = _text.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _creditsButton.image.color = buttonHoverColor;

        if (_text)
            _text.color = textHoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _creditsButton.image.color = Color.white;

        if (_text)
            _text.color = originalTextColor;
    }
}