using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class DestroyableObjectStats : CharacterStats
{
    [SerializeField] private UnityEvent OnDie;
    private Spawner spawner;
    private MeshDestroy meshDestroy;
    
    private void Start()
    {
        meshDestroy = GetComponent<MeshDestroy>();
        spawner = GetComponent<Spawner>();
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
        spawner?.Spawn();
        meshDestroy.DestroyMesh();
    }
}
