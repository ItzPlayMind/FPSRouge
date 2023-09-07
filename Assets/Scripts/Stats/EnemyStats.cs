using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{

    private void Start()
    {
        if (!IsOwner)
            return;

    }

    public override void Die()
    {
        base.Die();
        networkObject?.Despawn(true);
    }
}
