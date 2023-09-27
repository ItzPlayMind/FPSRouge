using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InteractObjective : Objective
{
    [SerializeField] private List<NetworkInteractable> interactables = new List<NetworkInteractable>();
    [System.Serializable]
    private class NetworkInteractable
    {
        public WorldInteractable interactable;
        public Transform transform;
    }

    private int counter = 0;

    public override void Setup()
    {
        if(IsOwner)
            SetupServerRpc();
    }

    [ServerRpc]
    private void SetupServerRpc()
    {
        ulong[] ids = new ulong[interactables.Count];
        int i = 0;
        foreach (var item in interactables)
        {
            var obj = Instantiate(item.interactable, item.transform.position, item.transform.rotation);
            obj.GetComponent<NetworkObject>().Spawn();
            ids[i] = obj.NetworkObjectId;
            i++;
        }
        SetupClientRpc(ids);
    }

    [ClientRpc]
    private void SetupClientRpc(ulong[] ids)
    {
        foreach (var item in ids)
        {
            var networkObj = GetNetworkObject(item);
            networkObj?.GetComponent<WorldInteractable>()?.OnInteract.AddListener(OnInteracted);
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
