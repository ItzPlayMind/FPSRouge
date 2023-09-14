using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    private static PlayerSpawn instance;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }

    public static PlayerSpawn Instance { get => instance; }

    [SerializeField] private GameObject playerPrefab;
    public void Spawn()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            //Destroy(gameObject);
            return;
        }

        foreach (var item in NetworkManager.Singleton.ConnectedClientsIds)
        {
            var player = Instantiate(playerPrefab, transform.position, transform.rotation);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(item);
        }
    }
}
