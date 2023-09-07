using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : MonoBehaviour
{
    public enum Hand
    {
        Off, Main
    }

    [SerializeField] private Transform offHandTransform;
    [SerializeField] private Transform mainHandTransform;

    [SerializeField] private Animator offhandAnimator;
    [SerializeField] private Animator mainhandAnimator;

    public Animator OffHandAnimator { get => offhandAnimator; }
    public Animator MainHandAnimator { get => mainhandAnimator; }

    public Object Instantiate(Object obj, Hand hand)
    {
        Transform spot = null;
        switch (hand)
        {
            case Hand.Off:
                spot = offHandTransform;
                break;
            case Hand.Main:
                spot = mainHandTransform;
                break;
        }
        return Instantiate(obj, spot);
    }

    public Item Instantiate(Item obj, Hand hand)
    {
        Transform spot = null;
        switch (hand)
        {
            case Hand.Off:
                spot = offHandTransform;
                if (obj.AnimatorController != null)
                    offhandAnimator.runtimeAnimatorController = obj.AnimatorController;
                break;
            case Hand.Main:
                spot = mainHandTransform;
                if (obj.AnimatorController != null)
                    mainhandAnimator.runtimeAnimatorController = obj.AnimatorController;
                break;
        }
        return Instantiate(obj, spot);
    }
}
