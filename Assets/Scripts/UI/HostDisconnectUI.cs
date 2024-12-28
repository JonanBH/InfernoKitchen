using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class HostDisconnectUI : MonoBehaviour
{
    [SerializeField] private Button playAgainButton;

    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;

        Hide();
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if(clientId == NetworkManager.ServerClientId)
        {
            // Server is Shutting down
            Show();
            return;
        }

        if(clientId == NetworkManager.Singleton.LocalClientId && NetworkManager.Singleton.DisconnectReason.IsNullOrEmpty())
        {
            Show();
            return;
        }
    }

    private void OnDestroy()
    {
        if (!NetworkManager.Singleton) return;

        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
