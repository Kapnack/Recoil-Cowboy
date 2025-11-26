using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button _creditsButton;

    private void Awake()
    {
        _creditsButton = gameObject.GetComponent<Button>();
    }
    
    public void OnPointerEnter(PointerEventData eventData) => _creditsButton.image.color = new Color(0.65f, 0.65f, 0.65f, 255);
    public void OnPointerExit(PointerEventData eventData) => _creditsButton.image.color = Color.white;
}