using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Utils;

public abstract class Weapon : Item
{
    [SerializeField] private float damage;
    public OnChangeValue<float> OnChangeDamage;
    public float Damage { get
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


    private float attackTimer = 0;

    public void Attack(Transform attacker)
    {
        if(attackTimer <= 0)
        {
            _Attack(attacker);
            attackTimer = AttackSpeed;
        }
    }

    private void Update()
    {
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;
    }

    protected abstract void _Attack(Transform attacker);

    public override void Use(Transform user)
    {
        Attack(user);
    }

}
