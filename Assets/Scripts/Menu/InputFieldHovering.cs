using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputFieldHovering : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public Color HoveringColor;

    private Color m_initialColor;

    void Start()
    {
        m_initialColor = GetComponent<Image>().color;
    }

    // Method triggered when the mouse cursor entered the input field area
    public void OnPointerEnter(PointerEventData pointerEvent)
    {
        GetComponent<Image>().color = HoveringColor;
    }

    // Method triggered when the mouse cursor exited the input field area
    public void OnPointerExit(PointerEventData pointerEvent)
    {
        GetComponent<Image>().color = m_initialColor;
    }
}
