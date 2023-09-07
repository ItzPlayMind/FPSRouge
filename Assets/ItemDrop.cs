using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemDrop : Interactable
{
    [SerializeField] private Item item;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        item.Instantiate(transform);
    }

    public override void Interact(PlayerController player)
    {
        Debug.Log("Interact!");
        PickupServerRpc(player.NetworkObjectId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void PickupServerRpc(ulong id)
    {
        PickupClientRpc(id);
    }

    [ClientRpc]
    private void PickupClientRpc(ulong id)
    {
        GetNetworkObject(id)?.GetComponent<WeaponManager>()?.ChangeItem(item, Hands.Hand.Main);
        Destroy();
    }

    public override void OnHover(PlayerController player)
    {
        //Debug.Log("Hover!");
    }
}
