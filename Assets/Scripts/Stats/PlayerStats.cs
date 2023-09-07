using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    [SerializeField] private float maxStamina = 100;
    [SerializeField] private float staminaRechargeRate = 20f;

    private float stamina;

    public float Stamina { get => stamina; }

    private float staminaTimer = 0;

    void Start()
    {
        if (!IsOwner)
            return;
        stamina = maxStamina;
        currentHealth.OnValueChanged = (float oldValue, float newValue) =>
        {
            PlayerUI.Instance.SetHealthBar(newValue/MaxHealth);
        };
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        if (staminaTimer > 0)
            staminaTimer -= Time.deltaTime;
        else if (stamina <= maxStamina)
            UpdateStamina(stamina + staminaRechargeRate * Time.deltaTime);
    }

    private void UpdateStamina(float stamina)
    {
        this.stamina = Mathf.Clamp(stamina, 0, maxStamina);
        PlayerUI.Instance.SetStaminaBar(this.stamina / maxStamina);
    }

    public bool HasStamina(float amount)
    {
        if (!IsOwner)
            return false;
        if (amount > stamina)
            return false;
        return true;
    }

    public void TakeStamina(float amount)
    {
        if (!IsOwner)
            return;
        if (!HasStamina(amount))
            return;
        UpdateStamina(stamina - amount);
        staminaTimer = 1f;
    }
}
