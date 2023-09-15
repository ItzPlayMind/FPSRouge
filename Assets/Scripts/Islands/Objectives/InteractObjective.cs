using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InteractObjective : Objective
{
    [SerializeField] private List<WorldInteractable> interactables = new List<WorldInteractable>();
    private int counter = 0;
    public override void Setup()
    {
        foreach (var item in interactables)
        {
            item.OnInteract.AddListener(OnInteracted);
        }
    }

    private void OnInteracted()
    {
        if (Finished)
            return;
        OnInteractedServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnInteractedServerRpc()
    {
        OnInteractedClientRpc();
    }

    [ClientRpc]
    private void OnInteractedClientRpc()
    {
        counter++;
        if(counter >= interactables.Count)
            Complete();
    }
}
