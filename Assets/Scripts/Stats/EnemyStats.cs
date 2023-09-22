using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    [SerializeField] private NetworkObject ragdoll;
    WeaponManager manager;

    protected override void _OnNetworkSpawn()
    {
        base._OnNetworkSpawn();
        manager = GetComponent<WeaponManager>();
    }

    public override void Die()
    {
        base.Die();
        SpawnManager.Instance.Spawn<Item>(manager.GetItem(Hands.Hand.Main).UID(), transform.position + Vector3.up);
        SpawnRagdollServerRpc();
        networkObject?.Despawn(true);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnRagdollServerRpc()
    {
        var activeRagdoll = Instantiate(ragdoll, transform.position, transform.rotation);
        activeRagdoll.Spawn(true);
    }
}
