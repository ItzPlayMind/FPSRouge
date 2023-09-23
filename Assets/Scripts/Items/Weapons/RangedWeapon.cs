using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ranged Weapon", menuName = "Items/Weapons/New Ranged Weapon")]
public class RangedWeapon : Weapon
{
    [SerializeField] private Projectile projectile;
    [SerializeField] private float force = 50f;
    protected override void _Attack(Transform usePoint, CharacterStats attacker)
    {
        RaycastHit hit;
        Vector3 point;
        if (Physics.Raycast(usePoint.position, usePoint.forward, out hit, AttackRange))
            point = hit.point;
        else
            point = usePoint.position + usePoint.forward * AttackRange;
        Vector3 dir = (point - AttackPoint.position).normalized;
        SpawnManager.Instance.SpawnProjectile(attacker.NetworkObjectId, AttackPoint.position, dir, force);
    }

    public virtual void OnProjectileSpawned(ulong spawnerID, Projectile projectile)
    {
        projectile.OnTargetHit += (GameObject target) =>
        {
            CharacterStats stats = target.GetComponent<CharacterStats>();
            if(stats != null)
            {
                stats.TakeDamage(Damage, DamageType, spawnerID);
            }
        };
    }

    public Projectile GetProjectile() => projectile;

}
