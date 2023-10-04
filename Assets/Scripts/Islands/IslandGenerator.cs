using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class IslandGenerator : NetworkBehaviour
{
    
    [SerializeField] private List<Island> islands = new List<Island>();
    [SerializeField] private Utils.Range<int> islandCount = new Utils.Range<int>(1, 5);
    [SerializeField] private Unity.AI.Navigation.NavMeshSurface navMeshSurface;

    private Vector3 maxSearchSize = new Vector3(100,100,100);
    private List<Island> spawnedIslands = new List<Island>();

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;

        GenerateIslandsServerRpc();
    }


    [ServerRpc]
    private void GenerateIslandsServerRpc()
    {
        int count = Random.Range(islandCount.min, islandCount.max);
        Island previousIsland = null;
        for (int i = 0; i < count; i++)
        {
            Island randomIsland = Instantiate(islands[Random.Range(0, islands.Count)]);
            Vector3 pos = GetRandomPosition(randomIsland);
            randomIsland.transform.position = pos;
            randomIsland.transform.eulerAngles = new Vector3(0, Random.Range(0, 359), 0);
            if (previousIsland != null)
                previousIsland.SetNext(randomIsland);
            var islandComp = randomIsland.GetComponent<NetworkObject>();
            islandComp.Spawn();
            spawnedIslands.Add(randomIsland);
            if(previousIsland != null)
                SetupPortalsClientRpc(previousIsland.NetworkObjectId, randomIsland.NetworkObjectId);
            previousIsland = randomIsland;
        }
        spawnedIslands[0].SpawnPlayers();
        UpdateNavMeshClientRpc();
    }

    [ClientRpc]
    private void SetupPortalsClientRpc(ulong previousID, ulong currentID)
    {
        Island previousIsland = GetNetworkObject(previousID).GetComponent<Island>();
        Island currentIsland = GetNetworkObject(currentID).GetComponent<Island>();

        previousIsland.End.SetTargetPortal(currentIsland.Start);
        currentIsland.Start.SetTargetPortal(previousIsland.End);
        previousIsland.End.OnEnter += currentIsland.Arrive;
    }

    [ClientRpc]
    private void UpdateNavMeshClientRpc()
    {
        navMeshSurface.BuildNavMesh();
    }

    private Vector3 GetRandomPosition(Island island)
    {
        Vector3 pos = Vector3.zero;
        int hitCount = 0;
        while (hitCount <= 100)
        {
            if (hitCount % 3 == 0)
                maxSearchSize += new Vector3(100,100,100);
            const float factor = 50;
            pos = new Vector3(Random.Range(-maxSearchSize.x / factor, maxSearchSize.x / factor),
                Random.Range(-maxSearchSize.y / factor, maxSearchSize.y / factor),
                Random.Range(-maxSearchSize.z / factor, maxSearchSize.z / factor))* factor;

            bool intersactsWithOther = false;
            foreach (var item in spawnedIslands)
            {
                Bounds islandBounds = new Bounds(pos, island.Size * 2);
                if (item.Bounds.Intersects(islandBounds))
                {
                    Debug.Log("Inside other island: " + pos + " " + item.transform.position);
                    hitCount++;
                    intersactsWithOther = true;
                    break;
                }
            }
            if(!intersactsWithOther)
                break;
        }
        return pos;
    }
}
