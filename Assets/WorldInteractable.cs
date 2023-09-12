using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldInteractable : Interactable
{
    [SerializeField] private bool onceOnly;
    [SerializeField] public UnityEvent OnInteract;

    private bool interacted;

    public override void Interact(PlayerController player, InteractionType type)
    {
        if (onceOnly && interacted)
            return;
        Debug.Log("Interacted with " + name);
        interacted = true;
        OnInteract?.Invoke();
    }

    public override void OnHover(PlayerController player)
    {

    }

    public override void OnHoverEnd(PlayerController player)
    {
        if (onceOnly && interacted)
            return;
        //TODO: Hide UI
    }

    public override void OnHoverStart(PlayerController player)
    {
        if (onceOnly && interacted)
            return;
        //TODO: Show UI
    }

}
