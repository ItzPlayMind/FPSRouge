using System.Collections;
using System.Collections.Generic;
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


    private float attackTimer = 0;

    public bool Attack(Transform usePoint, CharacterStats attacker)
    {
        if (attackTimer <= 0)
        {
            if (attacker is PlayerStats && !(attacker as PlayerStats).TakeStamina(StaminaUsage))
                return false;
            canUse = false;
            _Attack(usePoint, attacker);
            attackTimer = AttackSpeed;
            return true;
        }
        return false;
    }

    private void Update()
    {
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;
        else
            canUse = true;
    }

    protected abstract void _Attack(Transform usePoint, CharacterStats attacker);

    public override bool Use(Transform usePoint, CharacterStats user)
    {
        return Attack(usePoint, user);
    }

}
