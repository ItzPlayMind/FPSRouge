using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EventChest : Interactable
{
    [SerializeField] private Objective chestEvent;
    private Spawner spawner;
    private AnimationEventSender animationEventSender;
    private Animation anim;

    private Objective current;

    private void Start()
    {
        spawner = GetComponent<Spawner>();
        animationEventSender = GetComponent<AnimationEventSender>();
        anim = GetComponent<Animation>();
    }

    public override void _Interact(PlayerController player, InteractionType type)
    {
        Debug.Log("Interacted with Chest!");
        animationEventSender.OnAnimationEvent += () =>
        {
            Debug.Log("Spawn!");
            spawner.Spawn();
        }; 
        StartEventServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartEventServerRpc()
    {
        current = Instantiate(chestEvent,transform.position,transform.rotation);
        current.OnComplete += OpenClientRpc;
        StartEventClientRpc(current.NetworkObjectId);
    }

    [ClientRpc]
    private void StartEventClientRpc(ulong objectiveID)
    {
        current = GetNetworkObject(objectiveID).GetComponent<Objective>();
        current.Setup();
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
