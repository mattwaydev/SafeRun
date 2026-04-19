using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI texto;

    void Start()
    {
        texto = GetComponentInChildren<TextMeshProUGUI>();
        if (texto != null)
        {
            texto.outlineWidth = 0f;
            texto.outlineColor = Color.white;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (texto != null)
        {
            texto.outlineWidth = 0.2f;
            texto.outlineColor = Color.white;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (texto != null)
            texto.outlineWidth = 0f;
    }
}