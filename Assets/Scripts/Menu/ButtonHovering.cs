using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

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
            if (GetComponentsInChildren<Text>().Length > 0)
            {
                GetComponentsInChildren<Text>()[0].fontStyle = FontStyle.Bold;
            }
            if (GetComponentsInChildren<TextMeshProUGUI>().Length > 0)
            {
                GetComponentsInChildren<TextMeshProUGUI>()[0].fontStyle = FontStyles.Bold;
            }
        }
    }

    // Method triggered when the mouse cursor exited the button area
    public void OnPointerExit(PointerEventData pointerEvent)
    {
        if(m_button.IsInteractable())
        {
            if (GetComponentsInChildren<Text>().Length > 0)
            {
                GetComponentsInChildren<Text>()[0].fontStyle = FontStyle.Normal;
            }
            if (GetComponentsInChildren<TextMeshProUGUI>().Length > 0)
            {
                GetComponentsInChildren<TextMeshProUGUI>()[0].fontStyle = FontStyles.Normal;
            }
        }
    }
}
