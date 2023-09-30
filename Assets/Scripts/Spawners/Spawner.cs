using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static Utils;

public class Spawner : NetworkBehaviour
{
    [SerializeField] private Utils.Range<int> itemCount; 
    [SerializeField] private LootTable lootTable;
    [SerializeField] private float force = 5f;
    [SerializeField] private Range<Vector3> forceRange = new Range<Vector3>(new Vector3(-0.5f,1,-0.5f), new Vector3(0.5f, 1, 0.5f));

    public void Spawn()
    {
        SpawnServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnServerRpc()
    {
        int items = Random.Range(itemCount.min, itemCount.max);
        for (int i = 0; i < items; i++)
        {
            var loot = lootTable.Generate();
            if (loot is Weapon)
            {
                var weapon = loot as Weapon;
                int level = Random.Range(1, weapon.MaxLevel);
                Debug.Log(level);
                Weapon.Metadata data = new Weapon.Metadata(level);
                SpawnManager.Instance.Spawn(loot, data, transform.position, RandomVector3(forceRange.min, forceRange.max), force);
                continue;
            }
            SpawnManager.Instance.Spawn(loot, null, transform.position, RandomVector3(forceRange.min,forceRange.max), force);
        }
    }
}
