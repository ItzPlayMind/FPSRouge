using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NetworkUI : MonoBehaviour
{

    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button disconnectHostButton;
    [SerializeField] private Button disconnectClientButton;
    [SerializeField] private TMPro.TMP_InputField ipAddress;

    [SerializeField] private UnityEvent OnHost;
    [SerializeField] private UnityEvent OnClient;
    [SerializeField] private UnityEvent OnHostDisconnect;
    [SerializeField] private UnityEvent OnClientDisconnect;

    private void Start()
    {
        var unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        ipAddress.text = unityTransport.ConnectionData.Address;
        ipAddress.onValueChanged.AddListener((string value) =>
        {
            unityTransport.ConnectionData.Address = value.Trim();
        });
        hostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            OnHost?.Invoke();
            gameObject.SetActive(false);
        });
        clientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            OnClient?.Invoke();
            gameObject.SetActive(false);
        });
        disconnectHostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            OnHostDisconnect?.Invoke();
            gameObject.SetActive(true);
        });
        disconnectClientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            OnClientDisconnect?.Invoke();
            gameObject.SetActive(true);
        });
    }
}
