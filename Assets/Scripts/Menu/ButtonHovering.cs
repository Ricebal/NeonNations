using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHovering : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private Button m_button;

    void Start()
    {
        m_button = GetComponent<Button>();
    }

    // Method triggered when the mouse cursor entered the button area
    public void OnPointerEnter(PointerEventData pointerEvent)
    {
        if(m_button.IsInteractable())
        {
            GetComponentsInChildren<Text>()[0].fontStyle = FontStyle.Bold;
        }
    }

    // Method triggered when the mouse cursor exited the button area
    public void OnPointerExit(PointerEventData pointerEvent)
    {
        if(m_button.IsInteractable())
        {
            GetComponentsInChildren<Text>()[0].fontStyle = FontStyle.Normal;
        }
    }
}
