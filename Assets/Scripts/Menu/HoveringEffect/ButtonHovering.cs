using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHovering : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private Button m_button;

    private void Start()
    {
        m_button = GetComponent<Button>();
    }

    // Method triggered when the mouse cursor entered the button area
    public void OnPointerEnter(PointerEventData pointerEvent)
    {
        if (m_button.IsInteractable())
        {
            GetComponentsInChildren<TextMeshProUGUI>()[0].fontStyle = FontStyles.Bold;
        }
    }

    // Method triggered when the mouse cursor exited the button area
    public void OnPointerExit(PointerEventData pointerEvent)
    {
        if (m_button.IsInteractable())
        {
            GetComponentsInChildren<TextMeshProUGUI>()[0].fontStyle = FontStyles.Normal;
        }
    }

    public void OnDisable()
    {
        GetComponentsInChildren<TextMeshProUGUI>()[0].fontStyle = FontStyles.Normal;
    }
}
