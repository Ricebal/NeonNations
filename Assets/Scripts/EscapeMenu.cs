using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapeMenu : MonoBehaviour
{
    public GameObject Canvas;
    public Button ResumeButton;
    public Button ExitButton;
    private bool m_paused = false;

    // Start is called before the first frame update
    void Start()
    {
        Canvas.gameObject.SetActive(false);
        ResumeButton.onClick.AddListener(TogglePause);
        ExitButton.onClick.AddListener(Application.Quit);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("escape")) {
            TogglePause();    
        }
    }

    void TogglePause() {
        m_paused = !m_paused;
        Canvas.gameObject.SetActive(m_paused);
    }
}
