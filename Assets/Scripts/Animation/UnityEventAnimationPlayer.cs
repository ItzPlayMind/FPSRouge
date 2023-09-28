using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityEventAnimationPlayer : MonoBehaviour
{
    private Animation anim;
    private void Start()
    {
        anim = GetComponent<Animation>();
    }
    public void Play()
    {
        anim.Play();
    }
}
