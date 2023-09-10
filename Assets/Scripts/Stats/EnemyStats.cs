using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    WeaponManager manager;

    protected override void _OnNetworkSpawn()
    {
        base._OnNetworkSpawn();
        manager = GetComponent<WeaponManager>();
    }

    public override void Die()
    {
        base.Die();
        SpawnManager.Instance.SpawnItemDrop(manager.GetItem(Hands.Hand.Main).UID, transform.position + Vector3.up);
        networkObject?.Despawn(true);
    }
}
