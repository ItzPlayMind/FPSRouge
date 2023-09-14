using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static Utils;
using System.Linq;

public class Island : NetworkBehaviour
{
    public bool Complete { get; private set; }
    public Island Next { get; private set; }

    [SerializeField] private bool spawnPlayers;

    [SerializeField] private GameObject portal;

    [SerializeField] private List<Objective> objectives = new List<Objective>();

    [SerializeField] private Range<int> objectiveCount = new Range<int>(1, 3);

    private List<Objective> currentObjectives = new List<Objective>();
    private int completeObjectiveCounter = 0;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
            return;

        Setup();
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
        }

        SetupServerRpc(objectiveIds);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetupServerRpc(int[] objectives)
    {
        SetupClientRpc(objectives);
        if(spawnPlayers)
            SceneManager.Instance.OnLoadComplete = () =>
            {
                PlayerSpawn.Instance.Spawn();
            };
    }

    [ClientRpc]
    private void SetupClientRpc(int[] objectives)
    {
        for (int i = 0; i < this.objectives.Count; i++)
        {
            if (objectives.Contains(i))
            {
                currentObjectives.Add(this.objectives[objectives[i]]);
                currentObjectives[i].OnComplete += OnObjectiveComplete;
                currentObjectives[i].Setup();
            }
            else
            {
                this.objectives[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnObjectiveComplete()
    {
        completeObjectiveCounter++;
        if(completeObjectiveCounter >= currentObjectives.Count)
        {
            portal.SetActive(true);
            Complete = true;
        }
    }
}
