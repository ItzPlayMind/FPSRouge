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
    [SerializeField] private EffectDrop effectDrop;

    public void Spawn<T>(string uid, object metaData, Vector3 position, Vector3 dir = default, float force = 0)
    {
        var type = typeof(T);
        if (type == typeof(Item))
            SpawnItemDrop(uid, metaData, position, dir, force);
        if (type == typeof(Material))
            SpawnMaterialDrop(uid, position, dir, force);
        if (type == typeof(Effect))
            SpawnEffectDrop(uid, position, dir, force);
    }

    public void Spawn<T>(T item, object metaData, Vector3 position, Vector3 dir = default, float force = 0)
    {
        if (item is Item)
            SpawnItemDrop((item as Item).UID(), metaData, position, dir, force);
        if (item is Material)
            SpawnMaterialDrop((item as Material).UID(), position, dir, force);
        if (item is Effect)
            SpawnEffectDrop((item as Effect).UID(), position, dir, force);
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

    private void SpawnEffectDrop(string materialUid, Vector3 position, Vector3 dir = default, float force = 0)
    {
        SpawnEffectDropServerRpc(materialUid, position, dir, force);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnEffectDropServerRpc(string materialUid, Vector3 pos, Vector3 dir, float force)
    {
        var drop = Instantiate(effectDrop, pos, Quaternion.identity);
        drop.GetComponent<NetworkObject>()?.Spawn();
        drop.GetComponent<Rigidbody>()?.AddForce(dir * force, ForceMode.Impulse);
        ApplyEffectToEffectDropClientRpc(drop.NetworkObjectId, materialUid);
    }

    [ClientRpc]
    private void ApplyEffectToEffectDropClientRpc(ulong dropId, string materialUid)
    {
        var obj = GetNetworkObject(dropId).GetComponent<EffectDrop>();
        if (obj != null)
        {
            var effect = ScriptableObjectManager.Instance.Get<Effect>(materialUid).Clone();
            Debug.Log(effect.UID());
            obj.SetEffect(effect);
            obj.Setup();
        }
    }

    private void SpawnItemDrop(string itemUid, object metaData, Vector3 position, Vector3 dir = default, float force = 0)
    {
        SpawnItemDropServerRpc(itemUid, JsonUtility.ToJson(metaData), position, dir, force);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnItemDropServerRpc(string itemUid, string metaData,Vector3 pos, Vector3 dir, float force)
    {
        var drop = Instantiate(itemdrop, pos, Quaternion.identity);
        drop.GetComponent<NetworkObject>()?.Spawn();
        drop.GetComponent<Rigidbody>()?.AddForce(dir * force, ForceMode.Impulse);
        ApplyItemToItemDropClientRpc(drop.NetworkObjectId, itemUid, metaData);
    }

    [ClientRpc]
    private void ApplyItemToItemDropClientRpc(ulong dropId, string itemUid, string metaData)
    {
        Debug.Log(itemUid + " " + metaData);
        var obj = GetNetworkObject(dropId).GetComponent<ItemDrop>();
        if (obj != null)
        {
            obj.SetItem(ScriptableObjectManager.Instance.Get<Item>(itemUid,metaData));
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
        var proj = GetNetworkObject(projID).GetComponent<Projectile>();
        var attacker = GetNetworkObject(attackerID);
        proj.SetSpawner(attacker.gameObject);
        ((RangedWeapon)attacker?.GetComponent<WeaponManager>()?.GetItem(Hands.Hand.Main))?.OnProjectileSpawned(attackerID, proj);
    }

    public NetworkObject GetNetworkObjectById(ulong id) => GetNetworkObject(id);
}
