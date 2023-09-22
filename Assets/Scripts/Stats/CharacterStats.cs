using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static Utils;

public class CharacterStats : NetworkBehaviour
{

    [SerializeField] private float maxHealth;
    public OnChangeValue<float> OnChangeMaxHealth;

    public System.Action<float, ulong> OnTakeDamage;
    public System.Action<float, ulong> OnHeal;

    public float MaxHealth
    {
        get
        {
            float value = maxHealth;
            OnChangeMaxHealth?.Invoke(ref value);
            return value;
        }
    }

    protected NetworkVariable<float> currentHealth = new NetworkVariable<float>(0,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    protected NetworkObject networkObject;

    public bool isDead { get; private set; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
            return;
        networkObject = GetComponent<NetworkObject>();
        currentHealth.Value = MaxHealth;
        _OnNetworkSpawn();
    }

    protected virtual void _OnNetworkSpawn() { }

    public void TakeDamage(float damage, ulong netID)
    {
        TakeDamageServerRpc(damage, netID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TakeDamageServerRpc(float damage, ulong netID)
    {
        TakeDamageClientRpc(damage, netID);
    }

    [ClientRpc]
    private void TakeDamageClientRpc(float damage, ulong netID)
    {
        if (!IsOwner)
            return;
        damage = Mathf.Max(damage, 0);
        currentHealth.Value = Mathf.Clamp(currentHealth.Value - damage, 0, MaxHealth);
        OnTakeDamage?.Invoke(damage, netID);
        if (currentHealth.Value <= 0 && !isDead)
        {
            Die();
        }
    }

    public void Heal(float health, ulong netID)
    {
        HealServerRpc(health, netID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void HealServerRpc(float health, ulong netID)
    {
        HealClientRpc(health, netID);
    }

    [ClientRpc]
    private void HealClientRpc(float health, ulong netID)
    {
        if (!IsOwner)
            return;
        if (isDead)
            return;
        health = Mathf.Max(health, 0);
        currentHealth.Value = Mathf.Clamp(currentHealth.Value + health, 0, MaxHealth);
        OnHeal?.Invoke(health, netID);
    }

    public virtual void Die()
    {
        DieServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DieServerRpc()
    {
        DieClientRpc();
    }

    [ClientRpc]
    private void DieClientRpc()
    {
        isDead = true;
        Debug.Log(name + " died!");
    }
}
