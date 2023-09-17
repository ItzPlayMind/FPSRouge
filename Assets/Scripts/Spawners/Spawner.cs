using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
    [SerializeField] private Utils.Range<int> itemCount; 
    [SerializeField] private LootTable lootTable;

    public void Spawn()
    {
        SpawnServerRpc();
    }

    [ServerRpc]
    private void SpawnServerRpc()
    {
        int items = Random.Range(itemCount.min, itemCount.max);
        for (int i = 0; i < items; i++)
        {
            var loot = lootTable.Generate();
            SpawnManager.Instance.Spawn(loot, transform.position, Vector3.up + new Vector3(0.5f, 0, 0.5f), 5f);
        }
    }
}
