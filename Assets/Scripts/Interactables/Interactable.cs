using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class Interactable : NetworkBehaviour
{
    public enum InteractionType
    {
        Primary, Secondary
    }

    private NetworkObject networkObject;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        networkObject = GetComponent<NetworkObject>();
    }

    protected void Destroy() { if (IsOwner) networkObject.Despawn(); }

    public abstract void Interact(PlayerController player, InteractionType type);

    public abstract void OnHover(PlayerController player);

    public abstract void OnHoverStart(PlayerController player); 
    public abstract void OnHoverEnd(PlayerController player);

}
