using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static Utils;

public class CharacterStats : NetworkBehaviour
{

    [SerializeField] private float maxHealth;
    public OnChangeValue<float> OnChangeMaxHealth;
    public float MaxHealth
    {
        get
        {
            float value = maxHealth;
            OnChangeMaxHealth?.Invoke(ref value);
            return value;
        }
    }

    [SerializeField] private TMPro.TextMeshProUGUI debugHealthText;

    [SerializeField] private NetworkVariable<float> currentHealth = new NetworkVariable<float>(0,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public bool isDead { get; private set; }

    private void Start()
    {
        if (!IsOwner)
            return;

        currentHealth.Value = MaxHealth;
        currentHealth.OnValueChanged = (float oldValue, float newValue) =>
        {
            PlayerUI.Instance.SetDebugHealthText(newValue);
        };
    }

    public void TakeDamage(float damage)
    {
        TakeDamageServerRpc(damage);
    }

    [ServerRpc(RequireOwnership = false)]
    private void TakeDamageServerRpc(float damage)
    {
        TakeDamageClientRpc(damage);
    }

    [ClientRpc]
    private void TakeDamageClientRpc(float damage)
    {
        if (!IsOwner)
            return;
        damage = Mathf.Max(damage, 0);
        currentHealth.Value = Mathf.Clamp(currentHealth.Value - damage, 0, MaxHealth);
        if (currentHealth.Value <= 0 && !isDead)
        {
            Die();
        }
    }

    public void Heal(float health)
    {
        HealServerRpc(health);
    }

    [ServerRpc(RequireOwnership = false)]
    private void HealServerRpc(float health)
    {
        HealClientRpc(health);
    }

    [ClientRpc]
    private void HealClientRpc(float health)
    {
        if (!IsOwner)
            return;
        if (isDead)
            return;
        health = Mathf.Max(health, 0);
        currentHealth.Value = Mathf.Clamp(currentHealth.Value + health, 0, MaxHealth);
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
