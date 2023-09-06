using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : MonoBehaviour
{
    public enum Hand
    {
        Left, Right
    }

    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private Transform rightHandTransform;

    [SerializeField] private Animator lefthandAnimator;
    [SerializeField] private Animator righthandAnimator;

    public Animator LeftHandAnimator { get => lefthandAnimator; }
    public Animator RightHandAnimator { get => righthandAnimator; }

    public Object Instantiate(Object obj, Hand hand)
    {
        Transform spot = null;
        switch (hand)
        {
            case Hand.Left:
                spot = leftHandTransform;
                break;
            case Hand.Right:
                spot = rightHandTransform;
                break;
        }
        return Instantiate(obj, spot);
    }

    public Item Instantiate(Item obj, Hand hand)
    {
        Transform spot = null;
        switch (hand)
        {
            case Hand.Left:
                spot = leftHandTransform;
                if (obj.AnimatorController != null)
                    lefthandAnimator.runtimeAnimatorController = obj.AnimatorController;
                break;
            case Hand.Right:
                spot = rightHandTransform;
                if (obj.AnimatorController != null)
                    righthandAnimator.runtimeAnimatorController = obj.AnimatorController;
                break;
        }
        return Instantiate(obj, spot);
    }
}
