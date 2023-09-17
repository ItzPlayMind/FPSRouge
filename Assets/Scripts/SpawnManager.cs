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
    [SerializeField] private MaterialDrop materialDrop;

    public void Spawn<T>(string uid, Vector3 position, Vector3 dir = default, float force = 0)
    {
        var type = typeof(T);
        if (type == typeof(Item))
            SpawnItemDrop(uid, position, dir, force);
        if (type == typeof(Material))
            SpawnMaterialDrop(uid, position, dir, force);
    }

    public void Spawn<T>(T item, Vector3 position, Vector3 dir = default, float force = 0)
    {
        if (item is Item)
            SpawnItemDrop((item as Item).UID(), position, dir, force);
        if (item is Material)
            SpawnMaterialDrop((item as Material).UID(), position, dir, force);
    }

    private void SpawnMaterialDrop(string materialUid, Vector3 position, Vector3 dir = default, float force = 0)
    {
        SpawnMaterialDropServerRpc(materialUid, position, dir, force);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnMaterialDropServerRpc(string materialUid, Vector3 pos, Vector3 dir, float force)
    {
        var drop = Instantiate(materialDrop, pos, Quaternion.identity);
        drop.GetComponent<NetworkObject>()?.Spawn();
        drop.GetComponent<Rigidbody>()?.AddForce(dir * force, ForceMode.Impulse);
        ApplyMaterialToMaterialDropClientRpc(drop.NetworkObjectId, materialUid);
    }

    [ClientRpc]
    private void ApplyMaterialToMaterialDropClientRpc(ulong dropId, string materialUid)
    {
        var obj = GetNetworkObject(dropId).GetComponent<MaterialDrop>();
        if (obj != null)
        {
            var material = ScriptableObjectManager.Instance.Get<Material>(materialUid).Clone();
            Debug.Log(material.UID());
            obj.SetMaterial(material);
            obj.Setup();
        }
    }

    private void SpawnItemDrop(string itemUid, Vector3 position, Vector3 dir = default, float force = 0)
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
            obj.SetItem(ScriptableObjectManager.Instance.Get<Item>(itemUid).Clone());
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
