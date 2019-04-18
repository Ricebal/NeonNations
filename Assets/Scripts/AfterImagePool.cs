using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImagePool : MonoBehaviour
{

    //public CharacterControl myCharacterControl;
    public GameObject TargetObject;     //Set these manually to the character object you're copying
    // public Animator targetAnimator;   //Set these manually to the character object you're copying
    public GameObject PreFab;           //This is the PreFab you made in the scene. It's a parent transform with an animator and AfterImage script on it, with Armature and SkinnedMeshRenderer children
    private const int POOLSIZE = 4;
    private List<AfterImage> m_afterImages;
    public bool ShowImages = false;

    private const int INTERVAL = 0;

    private int m_time = 0;

    // Use this for initialization
    void Start()
    {
      
    }

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
        if(ShowImages) {
            if (m_time > INTERVAL) { m_time = 0; AddAfterImage(); }
        } else {
            if (m_time > INTERVAL && m_afterImages != null && m_afterImages.Count > 0) { m_time = 0; DeleteAfterImage(); }            
        }
    }

    void AddAfterImage()
    {
        for (int i = 0; i < POOLSIZE; i++)
        {
            // m_afterImages[i].myRenderer.enabled = ShowImages;

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
