using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class DestroyableObjectStats : CharacterStats
{
    [SerializeField] private UnityEvent OnDie;
    private MeshDestroy meshDestroy;
    
    private void Start()
    {
        meshDestroy = GetComponent<MeshDestroy>();
    }

    public override void Die()
    {
        base.Die();
        OnDie?.Invoke();
        if (IsServer)
            DestroyMeshClientRpc();
    }

    [ClientRpc]
    private void DestroyMeshClientRpc()
    {
        meshDestroy.DestroyMesh();
    }
}
