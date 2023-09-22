using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Chest : Interactable
{
    private Spawner spawner;
    private AnimationEventSender animationEventSender;
    private Animation anim;

    private void Start()
    {
        spawner = GetComponent<Spawner>();
        animationEventSender = GetComponent<AnimationEventSender>();
        anim = GetComponent<Animation>();
    }

    public override void Interact(PlayerController player, InteractionType type)
    {
        Debug.Log("Interacted with Chest!");
        animationEventSender.OnAnimationEvent += () =>
        {
            Debug.Log("Spawn!");
            spawner.Spawn();
        };
        OpenServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void OpenServerRpc() => OpenClientRpc();

    [ClientRpc]
    private void OpenClientRpc() => anim.Play();

    public override void OnHover(PlayerController player)
    {

    }

    public override void OnHoverEnd(PlayerController player)
    {
        
    }

    public override void OnHoverStart(PlayerController player)
    {
        
    }
}
