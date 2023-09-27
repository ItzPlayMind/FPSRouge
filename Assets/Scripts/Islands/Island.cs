using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static Utils;
using System.Linq;
using UnityEngine.Events;

public class Island : NetworkBehaviour
{
    [SerializeField] private Vector3 size;
    public Vector3 Size { get => size; }
    public bool Complete { get; private set; }
    public Island Next { get; private set; }

    [System.Serializable]
    private class IslandNetworkObjectTransform
    {
        public NetworkObject networkObject;
        public Transform[] transform;
    }
    [SerializeField] private List<IslandNetworkObjectTransform> islandNetworkObjects = new List<IslandNetworkObjectTransform>();

    [SerializeField] private bool spawnPlayers;

    [SerializeField] private IslandPortal startPortal;
    [SerializeField] private IslandPortal endPortal;

    [SerializeField] private List<Objective> objectives = new List<Objective>();

    [SerializeField] private Range<int> objectiveCount = new Range<int>(1, 3);

    [SerializeField] private UnityEvent OnObjectivesComplete;

    [SerializeField] private UnityEvent OnArrive;


    private bool isStarted = false;
    private List<Objective> currentObjectives = new List<Objective>();
    private int completeObjectiveCounter = 0;

    public IslandPortal Start { get => startPortal; }
    public IslandPortal End { get => endPortal; }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
            return;
        Setup();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnIslandNetworkObjectsServerRpc()
    {
        foreach (var item in islandNetworkObjects)
        {
            foreach (var positions in item.transform)
            {
                var obj = Instantiate(item.networkObject, positions);
                obj.Spawn();
            }
        }
    }

    public void SetNext(Island island)
    {
        if (Next == null)
            Next = island;
    }

    private void Setup()
    {
        int[] objectiveIds = new int[Random.Range(objectiveCount.min, Mathf.Min(objectives.Count, objectiveCount.max))];
        List<int> availableObjectiveIds = new List<int>();

        for (int i = 0; i < objectives.Count; i++)
            availableObjectiveIds.Add(i);

        for (int i = 0; i < objectiveIds.Length; i++)
        {
            objectiveIds[i] = availableObjectiveIds[Random.Range(0, availableObjectiveIds.Count)];
            availableObjectiveIds.Remove(i);

            Debug.Log(objectiveIds[i]);
        }
        SetupServerRpc(objectiveIds);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetupServerRpc(int[] objectives)
    {
        SetupClientRpc(objectives);
        if (spawnPlayers)
            SpawnPlayers();
    }

    [ClientRpc]
    private void SetupClientRpc(int[] objectiveIds)
    {
        for (int i = 0; i < this.objectives.Count; i++)
        {
            if (objectiveIds.Contains(i))
            {
                currentObjectives.Add(this.objectives[i]);
                currentObjectives[currentObjectives.Count-1].OnComplete += OnObjectiveComplete;
                currentObjectives[currentObjectives.Count - 1].Setup();
            }
            else
            {
                this.objectives[i].gameObject.SetActive(false);
            }
        }
    }

    public void Arrive()
    {
        if (isStarted)
            return;
        SpawnIslandNetworkObjectsServerRpc(); 
        ArrivedServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ArrivedServerRpc()
    {
        ArrivedClientRpc();
    }

    [ClientRpc]
    private void ArrivedClientRpc()
    {
        OnArrive?.Invoke();
        isStarted = true;
    }

    public void SpawnPlayers()
    {
        SceneManager.Instance.OnLoadComplete = () =>
        {
            PlayerSpawn.Instance.Spawn();
        };
        Arrive();
    }

    private void OnObjectiveComplete()
    {
        completeObjectiveCounter++;
        if(completeObjectiveCounter >= currentObjectives.Count)
        {
            endPortal.gameObject.SetActive(true);
            Complete = true;
            OnObjectivesComplete?.Invoke();
        }
    }

    private void OnDrawGizmosSelected()
    {
        var color = Gizmos.color;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, size);
        Gizmos.color = color;
    }
}
