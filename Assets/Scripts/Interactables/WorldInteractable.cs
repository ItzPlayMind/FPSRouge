using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldInteractable : Interactable
{
    [SerializeField] public UnityEvent OnInteract;
    private Animator animator;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        animator = GetComponent<Animator>();
    }

    public override void _Interact(PlayerController player, InteractionType type)
    {
        Debug.Log("Interacted with " + name);
        OnInteract?.Invoke();
        animator?.SetTrigger("Interact");
    }

    public override void OnHover(PlayerController player)
    {

    }

    public override void OnHoverEnd(PlayerController player)
    {
        //TODO: Hide UI
    }

    public override void OnHoverStart(PlayerController player)
    {
        //TODO: Show UI
    }

}
