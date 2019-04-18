using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImagePool : MonoBehaviour
{
    public GameObject TargetObject;     
    // public Animator targetAnimator;  // Uncomment for animations
    public GameObject PreFab;           
    public bool ShowImages = false;
    private const int POOLSIZE = 4;
    private List<AfterImage> m_afterImages;
    private const int INTERVAL = 0;
    private int m_time = 0;

    public void CreatePool() 
    {
        m_afterImages = new List<AfterImage>(POOLSIZE);        
        for (int i = 0; i < POOLSIZE; i++)
        {
            GameObject nextAfterImage = Instantiate(PreFab);

            m_afterImages.Add(nextAfterImage.GetComponent<AfterImage>());

            nextAfterImage.GetComponent<AfterImage>().TargetObject = TargetObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_time++;
        // If the playing is dashing show the images otherwise start fading them out.
        if(ShowImages) {
            if (m_time > INTERVAL) { m_time = 0; AddAfterImage(); }
        } else {
            if (m_time > INTERVAL && m_afterImages != null && m_afterImages.Count > 0) { m_time = 0; DeleteAfterImage(); }            
        }
    }

    void AddAfterImage()
    {
        // Find an inactive image and activate it
        for (int i = 0; i < POOLSIZE; i++)
        {
            if (!m_afterImages[i].Active) { m_afterImages[i].Activate(); break; }
        }
    }

    private void DeleteAfterImage() {
        AfterImage toDestroy = m_afterImages[0];
        if(toDestroy.Fade()){
            m_afterImages.Remove(m_afterImages[0]);
            toDestroy.Destroy();
        };
    }
}
