using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HoldButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public int IncrementalValue;

    public delegate void ValueChangedDelegate(HoldButton button);
    public event ValueChangedDelegate OnValueChanged;

    [SerializeField] private float m_incrementalCooldown = 0;
    private bool m_isPressed;
    private float m_timeElapsed;

    public void OnPointerDown(PointerEventData eventData)
    {
        m_isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_isPressed = false;
    }

    private void FixedUpdate()
    {
        if (m_isPressed && Time.time - m_timeElapsed >= m_incrementalCooldown)
        {
            OnValueChanged?.Invoke(this);
            m_timeElapsed = Time.time;
        }
    }
}
