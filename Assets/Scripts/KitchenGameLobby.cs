using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameLobby : MonoBehaviour
{
    public event EventHandler OnCreateLobbyStarted;
    public event EventHandler OnCreateLobbyFailed;
    public event EventHandler OnJoinStarted;
    public event EventHandler OnQuickJoinFailed;
    public event EventHandler OnJoinFailed;
    public event EventHandler<OnLobbyListChangeEventArgs> OnLobbyListChanged;

    public class OnLobbyListChangeEventArgs : EventArgs
    {
        public List<Lobby> lobbyList;
    }

    public static KitchenGameLobby Instance { get; private set; }

    private Lobby joinedLobby;

    private float heartBeatTimer = 0;
    private const float LOBBY_HEART_BEAT_TIMER_MAX = 10f;

    private float listLobbiesTimer = 0;
    private const float LIST_LOBBIES_TIMER_MAX = 5f;

    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";


    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeUnityAuthentication();
    }

    private async void InitializeUnityAuthentication()
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            //initializationOptions.SetProfile(UnityEngine.Random.Range(0, 1000).ToString());

            await UnityServices.InitializeAsync(initializationOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        try
        {
            OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, KitchenGameMultiplayer.MAX_PLAYER_AMOUNT, new CreateLobbyOptions
            {
                IsPrivate = isPrivate
            });

            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation);
            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions 
            { 
                Data = new Dictionary<string, DataObject> 
                {
                    {KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                }
            
            });


            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

            KitchenGameMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);

        }catch(LobbyServiceException exception)
        {
            OnCreateLobbyFailed.Invoke(this, EventArgs.Empty);
            Debug.Log(exception);
        }
    }

    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            // For counting the player amount, it ignores the host
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(KitchenGameMultiplayer.MAX_PLAYER_AMOUNT - 1);

            return allocation;
        }
        catch(RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelayByCode(string relayCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayCode);
            return joinAllocation;
        }
        catch (RelayServiceException e) 
        {
            Debug.Log(e);
            return default;
        }
    }

    public async void QuickJoin()
    {
        try
        {
            OnJoinStarted?.Invoke(this, EventArgs.Empty);
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            JoinAllocation joinAllocation = await JoinRelayByCode(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            KitchenGameMultiplayer.Instance.StartClient();
        }catch(LobbyServiceException exception)
        {
            Debug.Log(exception);
            OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    public Lobby GetLobby()
    {
        return joinedLobby;
    }

    public async void JoinWithCode(string lobbyCode)
    {
        try
        {
            OnJoinStarted?.Invoke(this, EventArgs.Empty);

            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            JoinAllocation joinAllocation = await JoinRelayByCode(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            KitchenGameMultiplayer.Instance.StartClient();
        }catch(LobbyServiceException e)
        {
            Debug.Log(e);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }
    public async void JoinWithId(string lobbyId)
    {
        try
        {
            OnJoinStarted?.Invoke(this, EventArgs.Empty);

            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            JoinAllocation joinAllocation = await JoinRelayByCode(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            KitchenGameMultiplayer.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Update()
    {
        HandleHeartBeat();
        HandlePeriodicListLobbies();
    }

    private void HandlePeriodicListLobbies()
    {
        if (joinedLobby != null ||
            !AuthenticationService.Instance.IsAuthorized ||
            SceneManager.GetActiveScene().name != Loader.Scene.LobbyScene.ToString())
        { 
            return; 
        }

        listLobbiesTimer -= Time.deltaTime;

        if (listLobbiesTimer < 0)
        {
            listLobbiesTimer = LIST_LOBBIES_TIMER_MAX;
            ListLobbies();
        }
    }

    private bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                }
            };

            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangeEventArgs
            {
                lobbyList = queryResponse.Results
            });

        }catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void DeleteLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);

                joinedLobby = null;
            }catch(LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }



    public async void LeaveLobby()
    {
        if (joinedLobby == null) return;

        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            joinedLobby = null;
        }catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void KickPlayer(string playerId)
    {
        if (!IsLobbyHost()) return;

        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void HandleHeartBeat()
    {
        if (IsLobbyHost())
        {
            heartBeatTimer -= Time.deltaTime;

            if(heartBeatTimer < 0)
            {
                heartBeatTimer = LOBBY_HEART_BEAT_TIMER_MAX;
                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }   
    }
}
