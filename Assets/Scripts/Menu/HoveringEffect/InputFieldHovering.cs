/**
 * Authors: David
 */

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputFieldHovering : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public Color HoveringColor;

    private Image m_image;
    private Color m_initialColor;

    private void Start()
    {
        m_image = GetComponent<Image>();
        m_initialColor = GetComponent<Image>().color;
    }

    // Method triggered when the mouse cursor entered the input field area
    public void OnPointerEnter(PointerEventData pointerEvent)
    {
        m_image.color = HoveringColor;
    }

    // Method triggered when the mouse cursor exited the input field area
    public void OnPointerExit(PointerEventData pointerEvent)
    {
        m_image.color = m_initialColor;
    }
}
