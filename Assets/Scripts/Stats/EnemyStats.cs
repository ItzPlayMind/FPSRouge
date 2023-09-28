using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    [SerializeField] private NetworkObject ragdoll;
    [SerializeField] private UIBar healthbar;
    WeaponManager manager;

    private void Start()
    {
        currentHealth.OnValueChanged += (float previous, float current) => OnHealthChange();
    }

    protected override void _OnNetworkSpawn()
    {
        base._OnNetworkSpawn();
        manager = GetComponent<WeaponManager>();
    }
    private void OnHealthChange() => healthbar.SetBar(currentHealth.Value / MaxHealth);

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
