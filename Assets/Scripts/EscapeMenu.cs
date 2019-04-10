using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapeMenu : MonoBehaviour
{
    public GameObject Canvas;
    public Button ExitButton;
    private bool m_paused = false;

    // Start is called before the first frame update
    void Start()
    {
        Canvas.gameObject.SetActive(false);
        ExitButton.onClick.AddListener(() => Application.Quit());
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("escape")) {
            m_paused = !m_paused;
            Canvas.gameObject.SetActive(m_paused);
        }
    }
}
