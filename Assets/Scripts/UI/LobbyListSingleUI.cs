using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyText;

    private Button joinLobbyButton;
    private Lobby lobby;

    private void Awake()
    {
        joinLobbyButton = GetComponent<Button>();
        joinLobbyButton.onClick.AddListener(() =>
        {
            KitchenGameLobby.Instance.JoinWithId(lobby.Id);
        });
    }

    public void SetLobby(Lobby lobby)
    {
        this.lobby = lobby;

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        lobbyText.text = $"{lobby.Name} - {lobby.Players.Count}/{lobby.MaxPlayers}";
    }
}
