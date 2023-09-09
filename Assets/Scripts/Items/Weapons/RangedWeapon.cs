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
        SpawnManager.Instance.SpawnProjectile(attacker.NetworkObjectId, usePoint.position, usePoint.forward, force);
    }

    public virtual void OnProjectileSpawned(Projectile projectile)
    {
        projectile.OnTargetHit += (target) =>
        {
            Debug.Log(target.name + " was hit!");
        };
    }

    public Projectile GetProjectile() => projectile;

}
