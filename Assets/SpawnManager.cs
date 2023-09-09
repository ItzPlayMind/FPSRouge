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
            Destroy(instance.gameObject);
        instance = this;
    }

    public static SpawnManager Instance { get => instance; }

    [SerializeField] private ItemDrop itemdrop;

    public ulong SpawnItemDrop(Vector3 position, Vector3 force)
    {
        var drop = Instantiate(itemdrop, position, Quaternion.identity);
        drop.GetComponent<NetworkObject>()?.Spawn();
        drop.GetComponent<Rigidbody>()?.AddForce(force, ForceMode.Impulse);
        return drop.NetworkObjectId;
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
        ((RangedWeapon)GetNetworkObject(attackerID)?.GetComponent<WeaponManager>()?.GetItem(Hands.Hand.Main))?.OnProjectileSpawned(GetNetworkObject(projID).GetComponent<Projectile>());
    }

    public NetworkObject GetNetworkObjectById(ulong id) => GetNetworkObject(id);
}
