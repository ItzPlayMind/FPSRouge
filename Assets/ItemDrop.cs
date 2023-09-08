using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemDrop : Interactable
{
    [SerializeField] private Item item;

    public void SetItem(Item item) => this.item = item;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        item?.Instantiate(transform);
    }

    public void Setup()
    {
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
        var networkOBJ = GetNetworkObject(id);
        if (networkOBJ != null)
        {
            var weaponManager = networkOBJ.GetComponent<WeaponManager>(); 
            if (weaponManager != null)
            {
                var dropID = SpawnManager.Instance.SpawnItemDrop(networkOBJ.transform.position+Vector3.up, networkOBJ.transform.forward * 10f);
                weaponManager.ApplyItemToItemDropClientRpc(Hands.Hand.Main, dropID);
            }
        }
        PickupClientRpc(id);
    }

    [ClientRpc]
    private void PickupClientRpc(ulong id)
    {
        var networkOBJ = GetNetworkObject(id);
        if (networkOBJ != null)
        {
            var weaponManager = networkOBJ.GetComponent<WeaponManager>();
            if (weaponManager != null)
            {
                weaponManager.ChangeItem(item, Hands.Hand.Main);
            }
        }
        Destroy();
    }

    public override void OnHover(PlayerController player)
    {
        //Debug.Log("Hover!");
    }
}
