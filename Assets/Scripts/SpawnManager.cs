using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    private static SpawnManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }

    public static SpawnManager Instance { get => instance; }

    [SerializeField] private ItemDrop itemdrop;

    public void SpawnItemDrop(string itemUid, Vector3 position, Vector3 dir = default, float force = 0)
    {
        SpawnItemDropServerRpc(itemUid, position, dir, force);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnItemDropServerRpc(string itemUid,Vector3 pos, Vector3 dir, float force)
    {
        var drop = Instantiate(itemdrop, pos, Quaternion.identity);
        drop.GetComponent<NetworkObject>()?.Spawn();
        drop.GetComponent<Rigidbody>()?.AddForce(dir * force, ForceMode.Impulse);
        ApplyItemToItemDropClientRpc(drop.NetworkObjectId, itemUid);
    }

    [ClientRpc]
    private void ApplyItemToItemDropClientRpc(ulong dropId, string itemUid)
    {
        Debug.Log(itemUid);
        var obj = GetNetworkObject(dropId).GetComponent<ItemDrop>();
        if (obj != null)
        {
            obj.SetItem(ItemManager.Instance.GetItem(itemUid).Clone());
            obj.Setup();
        }
    }

    public void SpawnProjectile(ulong id, Vector3 pos, Vector3 dir, float force)
    {
        SpawnProjectileServerRpc(id, pos, dir, force);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnProjectileServerRpc(ulong objID, Vector3 pos, Vector3 dir, float force)
    {
        var obj =  GetNetworkObject(objID);
        if(obj != null)
        {
            var weaponManager = obj.GetComponent<WeaponManager>();
            if(weaponManager != null)
            {
                var proj = Instantiate((weaponManager.GetItem(Hands.Hand.Main) as RangedWeapon).GetProjectile(),pos,Quaternion.LookRotation(dir));
                proj.GetComponent<NetworkObject>().Spawn();
                proj.AddForce(proj.transform.forward * force);
                SpawnProjectileClientRpc(objID, proj.NetworkObjectId, Utils.GetSendClientList(obj.OwnerClientId));
            }
        }
    }

    [ClientRpc]
    private void SpawnProjectileClientRpc(ulong attackerID, ulong projID, ClientRpcParams clientRpcParams = default)
    {
        ((RangedWeapon)GetNetworkObject(attackerID)?.GetComponent<WeaponManager>()?.GetItem(Hands.Hand.Main))?.OnProjectileSpawned(attackerID,GetNetworkObject(projID).GetComponent<Projectile>());
    }

    public NetworkObject GetNetworkObjectById(ulong id) => GetNetworkObject(id);
}
