using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public delegate void OnChangeValue<T>(ref T value);

    [SerializeField] private float damage;
    public OnChangeValue<float> OnChangeDamage;
    public float Damage { get
        {
            float value = damage;
            OnChangeDamage?.Invoke(ref damage);
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
            OnChangeAttackSpeed?.Invoke(ref attackSpeed);
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



}
