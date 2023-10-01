using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static Utils;

public abstract class Weapon : Item
{

    [System.Serializable]
    public class WeaponLevel
    {
        public GameObject gfx;
        public Effect offhandEffect;
        public float damage;
        public DamageType damageType;
        public float attackSpeed;
        public float attackRange;
        public float staminaUsage;
    }

    [System.Serializable]
    public class Metadata
    {
        public int level = 1;

        public Metadata(int level)
        {
            this.level = level;
        }
    }

    public System.Action<CharacterStats, Weapon> OnHit;

    private Transform attackPoint;
    public Transform AttackPoint { get => attackPoint; }

    private ParticleSystem trail = null;
    public ParticleSystem Trail { get => trail; }


    [SerializeField] private List<WeaponLevel> levels = new List<WeaponLevel>();
    [SerializeField] private int level = 1;
    public int Level { get => level; }
    public int MaxLevel { get => levels.Count; }

    public OnChangeValue<float> OnChangeDamage;
    public float Damage
    {
        get
        {
            float value = levels[level-1].damage;
            OnChangeDamage?.Invoke(ref value);
            return value;
        }
    }

    
    public OnChangeValue<DamageType> OnChangeDamageType;
    public DamageType DamageType
    {
        get
        {
            DamageType value = levels[level - 1].damageType;
            OnChangeDamageType?.Invoke(ref value);
            return value;
        }
    }

    
    public OnChangeValue<float> OnChangeAttackSpeed;
    public float AttackSpeed
    {
        get
        {
            float value = levels[level - 1].attackSpeed;
            OnChangeAttackSpeed?.Invoke(ref value);
            return value;
        }
    }

    
    public OnChangeValue<float> OnChangeAttackRange;
    public float AttackRange
    {
        get
        {
            float value = levels[level - 1].attackRange;
            OnChangeAttackRange?.Invoke(ref value);
            return value;
        }
    }

    
    public float StaminaUsage
    {
        get
        {
            return levels[level - 1].staminaUsage;
        }
    }

    public override GameObject Instantiate(Transform transform, WeaponManager manager = null)
    {
        offhandEffect = levels[level - 1].offhandEffect?.Clone();
        offhandEffect?.SetLevel(level);
        offhandEffect?.Setup();
        if (Active != null)
        {
            Destroy(Active);
            Active = null;
        }
        Active = GameObject.Instantiate(levels[level-1].gfx, transform);
        OnInstantiate(transform, manager);
        if (manager != null)
            SetupOnEquip(manager);
        return Active;
    }

    protected override void OnInstantiate(Transform transform, WeaponManager manager)
    {
        attackPoint = Active.transform.Find("AttackPoint");
        trail = Active.transform.Find("Trail")?.GetComponent<ParticleSystem>();
    }

    public override bool CanUse(CharacterStats user)
    {
        return (user is PlayerStats && (user as PlayerStats).HasStamina(StaminaUsage)) || user is not PlayerStats;
    }

    public void Attack(Transform usePoint, CharacterStats attacker)
    {
        _Attack(usePoint, attacker);
    }

    protected abstract void _Attack(Transform usePoint, CharacterStats attacker);

    protected override void _Use(Transform usePoint, CharacterStats user)
    {
        Attack(usePoint, user);
    }

    public override object GetMetadata() => new Metadata(level);
    public override void FromMetadata(object metaData)
    {
        if (metaData is not Metadata)
            return;
        var data = metaData as Metadata;
        level = data.level;
    }
}
