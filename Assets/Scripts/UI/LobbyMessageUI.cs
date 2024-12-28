using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class LobbyMessageUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    private void Start()
    {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame += KitchenGameMultiplayer_OnFailedToJoinGame;
        KitchenGameLobby.Instance.OnCreateLobbyStarted += KitchenGameLobby_OnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed += KitchenGameLobby_OnCreateLobbyFailed;
        KitchenGameLobby.Instance.OnJoinStarted += KitchenGameLobby_OnJoinStarted;
        KitchenGameLobby.Instance.OnJoinFailed += KitchenGameLobby_OnJoinFailed;
        KitchenGameLobby.Instance.OnQuickJoinFailed += KitchenGameLobby_OnQuickJoinFailed;

        Hide();
    }


    private void KitchenGameLobby_OnQuickJoinFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Could not find a lobby to quick join!");
    }

    private void KitchenGameLobby_OnJoinFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to join lobby!");
    }

    private void KitchenGameLobby_OnJoinStarted(object sender, System.EventArgs e)
    {
        ShowMessage("Joining lobby...");
    }

    private void KitchenGameLobby_OnCreateLobbyFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to create lobby!");
    }

    private void KitchenGameLobby_OnCreateLobbyStarted(object sender, System.EventArgs e)
    {
        ShowMessage("Creating lobby...");
    }

    private void ShowMessage(string message)
    {
        messageText.text = message;
        Show();
    }

    private void KitchenGameMultiplayer_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        if (NetworkManager.Singleton.DisconnectReason.IsNullOrEmpty())
        {
            ShowMessage("Failed to connect");
        }
        else
        {
            messageText.text = NetworkManager.Singleton.DisconnectReason;
        }
        
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameMultiplayer_OnFailedToJoinGame;
        KitchenGameLobby.Instance.OnCreateLobbyStarted -= KitchenGameLobby_OnCreateLobbyStarted;
        KitchenGameLobby.Instance.OnCreateLobbyFailed -= KitchenGameLobby_OnCreateLobbyFailed;
        KitchenGameLobby.Instance.OnJoinStarted -= KitchenGameLobby_OnJoinStarted;
        KitchenGameLobby.Instance.OnJoinFailed -= KitchenGameLobby_OnJoinFailed;
        KitchenGameLobby.Instance.OnQuickJoinFailed -= KitchenGameLobby_OnQuickJoinFailed;
    }
}
