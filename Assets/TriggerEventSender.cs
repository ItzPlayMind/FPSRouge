using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEventSender : MonoBehaviour
{
    public UnityEvent<Collider> OnTrigger;

    private void OnTriggerEnter(Collider other)
    {
        OnTrigger?.Invoke(other);
    }
}
