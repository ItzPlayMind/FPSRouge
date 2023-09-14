using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : NetworkBehaviour
{
    private static SceneManager instance;

    public System.Action<float> OnLoadProgress;
    public System.Action OnLoadComplete;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
        DontDestroyOnLoad(gameObject);
        if (IsServer)
            GetComponent<NetworkObject>().Spawn();
    }

    public static SceneManager Instance { get => instance; }

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private UIBar loadingBar;

    public void LoadSceneForAll(string level)
    {
        playersReady = 0;
        LoadSceneForAllServerRpc(level);
    }

    [ServerRpc(RequireOwnership = false)]
    private void LoadSceneForAllServerRpc(string level)
    {
        Load(level);
    }

    [ClientRpc]
    private void LoadingScreenClientRpc(bool value)
    {
        loadingScreen.SetActive(value);
    }

    private int playersReady = 0;

    public void Load(string level)
    {
        LoadingScreenClientRpc(true);
        UpdateBarClientRpc(0);
        NetworkManager.Singleton.SceneManager.LoadScene(level, LoadSceneMode.Single);
        NetworkManager.Singleton.SceneManager.OnLoadComplete += SceneLoadComplete;
    }

    [ClientRpc]
    private void UpdateBarClientRpc(float value)
    {
        OnLoadProgress?.Invoke(value);
        loadingBar.SetBar(value);
    }

    private void SceneLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        playersReady++;
        int playerCount = NetworkManager.Singleton.ConnectedClients.Count;
        UpdateBarClientRpc((float)playersReady / playerCount);
        if (playersReady >= playerCount)
        {
            LoadingScreenClientRpc(false);
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= SceneLoadComplete;
            OnLoadComplete?.Invoke();
        }
    }
}
