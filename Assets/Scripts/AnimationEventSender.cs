using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender : MonoBehaviour
{
    public System.Action OnAnimationEvent;

    public void OnAnimationEventRecieved() => OnAnimationEvent?.Invoke();
}
