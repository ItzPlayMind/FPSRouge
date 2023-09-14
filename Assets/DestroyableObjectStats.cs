using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DestroyableObjectStats : CharacterStats
{
    private MeshDestroy meshDestroy;

    private void Start()
    {
        meshDestroy = GetComponent<MeshDestroy>();
    }

    public override void Die()
    {
        base.Die();
        //TODO: Spawn Materials
        if(IsServer)
            DestroyMeshClientRpc();
    }

    [ClientRpc]
    private void DestroyMeshClientRpc()
    {
        meshDestroy.DestroyMesh();
    }
}
