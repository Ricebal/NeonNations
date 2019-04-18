using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    // Uncomment when player model with animation.
    // public Animator myAnimator;

    public Renderer MyRenderer;

    public Animator TargetAnimator;
    public GameObject TargetObject;

    private float m_time;
    private float m_intensity;
    private float m_pow;
    private const float TIME_MAX = 45;

    public bool Active;

	// Use this for initialization
	void Start ()
    {
		//TargetObject = 
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (m_time > 0) { m_time--; Active = true; m_intensity = (m_time / TIME_MAX) * 10 * m_pow; UpdateRenderer(); }//transform.localScale *= 1.03f; }
        else { Active = false; m_intensity = 0; }
	}

    void UpdateRenderer()
    {
        MyRenderer.material.SetFloat("_Intensity", m_intensity);
        MyRenderer.material.SetFloat("_MKGlowPower", m_intensity);
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    public bool Fade() {
        Color color = MyRenderer.material.color;
        if(color.a > 0){
            MyRenderer.material.color = new Color(color.r, color.g, color.b, color.a - 0.15f);
            return false;
        } else {
            MyRenderer.material.color = new Color(color.r, color.g, color.b, 0);
        }

        return true;
    }

    public void Activate()
    {
        Active = true;
        transform.position = TargetObject.transform.position;
        transform.localScale = TargetObject.transform.lossyScale;
        transform.rotation = TargetObject.transform.rotation;


        // Uncomment this when we add a player model with animations.
        // myAnimator.Play(TargetAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, TargetAnimator.GetCurrentAnimatorStateInfo(0).normalizedm_time);
        // //myAnimator.Play(myCharacterControl.animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        // //myAnimator.no

        // foreach(AnimatorControllerParameter param in TargetAnimator.parameters)
        // {
        //     if (param.type == AnimatorControllerParameterType.Float)
        //     {
        //         myAnimator.SetFloat(param.name, TargetAnimator.GetFloat(param.name));
        //     }
        //     if (param.type == AnimatorControllerParameterType.Int)
        //     {
        //         myAnimator.SetInteger(param.name, TargetAnimator.GetInteger(param.name));
        //     }
        // }

        // myAnimator.speed = 0;


        m_time = TIME_MAX + 1;
        Update();
    }

}
