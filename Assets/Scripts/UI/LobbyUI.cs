using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private TMPro.TextMeshProUGUI playerIdText;
    [SerializeField] private Transform playerList;

    Dictionary<ulong, GameObject> playerIds = new Dictionary<ulong, GameObject>();

    public void StartLoading()
    {
        SceneManager.Instance.LoadSceneForAll("SampleScene");
    }

    private void Start()
    {
        foreach (var item in NetworkManager.Singleton.ConnectedClientsIds)
        {
            AddToPlayerList(item);
        }

        NetworkManager.Singleton.OnClientConnectedCallback += AddToPlayerList;
        NetworkManager.Singleton.OnClientDisconnectCallback += RemoveFromPlayerList;
    }

    private void AddToPlayerList(ulong id)
    {
        var idText = Instantiate(playerIdText, playerList);
        idText.text = id == NetworkManager.Singleton.LocalClientId ?
            "<color=red>" + id.ToString() + "</color>" :
            id.ToString();
        playerIds.Add(id, idText.gameObject);
    }

    private void RemoveFromPlayerList(ulong id)
    {
        Destroy(playerIds[id]);
    }
}
