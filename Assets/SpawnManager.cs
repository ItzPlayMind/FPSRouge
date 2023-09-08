using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private static SpawnManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(instance.gameObject);
        instance = this;
    }

    public static SpawnManager Instance { get => instance; }

    [SerializeField] private ItemDrop itemdrop;

    public ulong SpawnItemDrop(Vector3 position, Vector3 force)
    {
        var drop = Instantiate(itemdrop, position, Quaternion.identity);
        drop.GetComponent<NetworkObject>()?.Spawn();
        drop.GetComponent<Rigidbody>()?.AddForce(force, ForceMode.Impulse);
        return drop.NetworkObjectId;
    }
}
