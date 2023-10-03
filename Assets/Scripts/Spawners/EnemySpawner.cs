using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private EnemySpawnList enemySpawnList;
    [SerializeField] private float spawnRange = 5f;
    [SerializeField] private Utils.Range<int> enemyAmount = new Utils.Range<int>(1, 5);

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;
        StartCoroutine(SpawnDelay(1));
    }

    IEnumerator SpawnDelay(float time)
    {
        yield return new WaitForSeconds(time);
        SpawnEnemiesServerRpc();
    }

    [ServerRpc]
    private void SpawnEnemiesServerRpc()
    {
        int amount = Random.Range(enemyAmount.min, enemyAmount.max);
        int tries = 100;
        while(tries > 0 && amount > 0)
        {
            var enemy = enemySpawnList.enemies[Random.Range(0, enemySpawnList.enemies.Count)];
            Vector2 pos = Random.insideUnitCircle;
            var randomPos = transform.position + Vector3.up * 2f + new Vector3(pos.x,0,pos.y) * spawnRange;
            RaycastHit hit;
            if (Physics.Raycast(randomPos, Vector3.down, out hit, 10))
            {
                var rot = new Vector3(0, Random.Range(0, 360), 0);
                var spawnedEnemy = Instantiate(enemy, hit.point, Quaternion.Euler(rot));
                spawnedEnemy.GetComponent<NetworkObject>().Spawn();
                amount--;
            }
            else
                tries--;
        }
    }

    private void OnDrawGizmosSelected()
    {
        var color = Gizmos.color;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRange);
        Gizmos.color = color;
    }
}
