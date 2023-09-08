using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static Utils;

public abstract class Weapon : Item
{
    [SerializeField] private float damage;
    public OnChangeValue<float> OnChangeDamage;
    public float Damage
    {
        get
        {
            float value = damage;
            OnChangeDamage?.Invoke(ref value);
            return value;
        }
    }

    [SerializeField] private float attackSpeed;
    public OnChangeValue<float> OnChangeAttackSpeed;
    public float AttackSpeed
    {
        get
        {
            float value = attackSpeed;
            OnChangeAttackSpeed?.Invoke(ref value);
            return value;
        }
    }

    [SerializeField] private float attackRange;
    public OnChangeValue<float> OnChangeAttackRange;
    public float AttackRange
    {
        get
        {
            float value = attackRange;
            OnChangeAttackRange?.Invoke(ref value);
            return value;
        }
    }

    [SerializeField] private float staminaUsage;
    public float StaminaUsage
    {
        get
        {
            return staminaUsage;
        }
    }

    public override bool CanUse(CharacterStats user)
    {
        return user is PlayerStats && (user as PlayerStats).HasStamina(StaminaUsage);
    }

    public void Attack(Transform usePoint, CharacterStats attacker)
    {
        _Attack(usePoint, attacker);
    }

    protected abstract void _Attack(Transform usePoint, CharacterStats attacker);

    public override void Use(Transform usePoint, CharacterStats user)
    {
        Attack(usePoint, user);
    }
}
