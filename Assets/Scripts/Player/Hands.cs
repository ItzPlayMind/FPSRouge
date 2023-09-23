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
    [SerializeField] private AnimationEventSender attackEventSender;
    [SerializeField] private Animator mainhandAnimator;

    public Animator MainHandAnimator { get => mainhandAnimator; }
    public AnimationEventSender AttackEventSender { get => attackEventSender; }

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

    public Item Instantiate(Item obj, Hand hand, WeaponManager manager, bool overrideAnimators = true)
    {
        Transform spot = null;
        switch (hand)
        {
            case Hand.Off:
                spot = offHandTransform;
                break;
            case Hand.Main:
                spot = mainHandTransform;
                if (obj.AnimatorController != null && overrideAnimators)
                    mainhandAnimator.runtimeAnimatorController = obj.AnimatorController;
                break;
        }
        obj.Instantiate(spot, manager);
        return obj;
    }
}
