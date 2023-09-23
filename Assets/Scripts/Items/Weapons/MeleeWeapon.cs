using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Melee Weapon", menuName = "Items/Weapons/New Melee Weapon")]
public class MeleeWeapon : Weapon
{
    [SerializeField] private Vector3 hitBoxSize;
    public Vector3 Hitbox { get => hitBoxSize; }
    protected override void _Attack(Transform usePoint, CharacterStats attacker)
    {
        var hits = Physics.BoxCastAll(usePoint.position, hitBoxSize / 2, usePoint.transform.forward, usePoint.rotation, AttackRange);
        foreach (var item in hits)
        {
            var stats = item.transform.GetComponent<CharacterStats>();
            if (stats != null)
            {
                if (stats == attacker)
                    continue;
                Debug.Log("Hit " + item.transform.name);
                stats.TakeDamage(Damage, DamageType, attacker.NetworkObjectId);
            }
        }
    }
}
