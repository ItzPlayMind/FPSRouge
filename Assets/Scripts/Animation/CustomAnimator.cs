using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomAnimator : MonoBehaviour
{
    private Animator animator;
    private bool animationIsRunning;
    public bool IsAnimating { get => animationIsRunning; }
    public void SetAnimator(Animator animator) => this.animator = animator;

    public void Play(string name, System.Action onAnimationFinish, float time = 1f)
    {
        animator.Play(name);
        var length = animator.GetCurrentAnimatorClipInfo(0).Length;
        animator.SetFloat("AnimationSpeed", length/time, 0, 0);
        animationIsRunning = true;
        StartCoroutine(WaitForAnimationEnd(name, onAnimationFinish));
    }

    IEnumerator WaitForAnimationEnd(string clip, System.Action onAnimationFinish)
    {
        yield return new WaitForEndOfFrame();
        while (animator?.GetCurrentAnimatorStateInfo(0).IsName(clip) ?? false) yield return null;
        animator?.SetFloat("AnimationSpeed", 1f, 0, 0);
        animationIsRunning = false;
        onAnimationFinish?.Invoke();
    }
}
