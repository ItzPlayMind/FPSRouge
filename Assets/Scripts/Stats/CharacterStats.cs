using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static Utils;

public class CharacterStats : NetworkBehaviour
{

    [SerializeField] private float maxHealth;
    public OnChangeValue<float> OnChangeMaxHealth;

    [SerializeField] private List<Modifiers> modifiers = new List<Modifiers>();

    [System.Serializable]
    private class Modifiers
    {
        public DamageType type;
        public float multiplier = 1;
    }

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

    public void TakeDamage(float damage, DamageType type, ulong netID)
    {
        TakeDamageServerRpc(damage, type, netID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TakeDamageServerRpc(float damage, DamageType type,  ulong netID)
    {
        TakeDamageClientRpc(damage, type, netID);
    }

    [ClientRpc]
    private void TakeDamageClientRpc(float damage, DamageType type, ulong netID)
    {
        if (!IsOwner)
            return;
        damage = Mathf.Max(damage, 0);
        var modifier = modifiers.Find(x => x.type == type);
        if(modifier != null)
            damage *= modifier.multiplier;
        currentHealth.Value = Mathf.Clamp(currentHealth.Value - damage, 0, MaxHealth);
        Debug.Log(name + " has taken Damage: " + damage);
        OnTakeDamage?.Invoke(damage, netID);
        if (currentHealth.Value <= 0 && !isDead)
        {
            Die();
        }
    }

    public void Heal(float health, DamageType type, ulong netID)
    {
        HealServerRpc(health, type, netID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void HealServerRpc(float health, DamageType type, ulong netID)
    {
        HealClientRpc(health, type, netID);
    }

    [ClientRpc]
    private void HealClientRpc(float health, DamageType type, ulong netID)
    {
        if (!IsOwner)
            return;
        if (isDead)
            return;
        health = Mathf.Max(health, 0);
        var modifier = modifiers.Find(x => x.type == type);
        if (modifier != null)
            health *= modifier.multiplier;
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
