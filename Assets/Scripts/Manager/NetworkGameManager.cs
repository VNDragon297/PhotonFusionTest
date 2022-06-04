using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Fusion;
using Fusion.Sockets;

public class NetworkGameManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [Header("Prefabs")]
    [SerializeField] private GameManager gameManagerPrefab;
    [SerializeField] private RoomPlayer roomPlayerPrefab;

    [Header("Level Manager")]
    [SerializeField] private LevelManager levelManager;

    public GameMode gameMode { get; private set; }
    private NetworkRunner networkRunner;

    private void Start()
    {
        Application.runInBackground = true;

        DontDestroyOnLoad(gameObject);

        // Load lobby scene here
    }

    public void SetCreateLobby() => gameMode = GameMode.Host;
    public void SetJoinLobby() => gameMode = GameMode.Client;

    public void JoinOrCreateLobby()
    {
        // If network runner exist, that means we're still in a lobby
        if (networkRunner != null)
            LeaveSession();

        // Add a gameobject to hold Network Runner
        GameObject gameObj = new GameObject("Session");
        DontDestroyOnLoad(gameObj);

        networkRunner = gameObj.AddComponent<NetworkRunner>();
        networkRunner.ProvideInput = gameMode != GameMode.Server;
        networkRunner.AddCallbacks(this);

        Debug.Log($"Created network runner attached to {gameObj.name} - Starting game");

        if(gameMode == GameMode.Host || gameMode == GameMode.Server)
        {
            networkRunner.StartGame(new StartGameArgs
            {
                GameMode = gameMode,
                SessionName = ServerInfo.LobbyName,
                SceneObjectProvider = levelManager,
                PlayerCount = ServerInfo.MaxUsers,
                DisableClientSessionCreation = true
            });
        }
        else if(gameMode == GameMode.Client)
        {
            networkRunner.StartGame(new StartGameArgs
            {
                GameMode = gameMode,
                SessionName = ServerInfo.LobbyName,
                SceneObjectProvider = levelManager,
                DisableClientSessionCreation = true
            });
        }
    }

    public void StartGame(SceneIndex index)
    {
        networkRunner.SetActiveScene((int)index);
    }

    public void LeaveSession()
    {
        if(networkRunner != null)
            networkRunner.Shutdown();
    }

    // Client call only
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to server");
    }

    // Client call only
    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("Disconnected from server");
    }

    // Client call only
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.LogWarning($"Connection failed: {reason}");
        LeaveSession();

        (string status, string message) = ConnectFailedReasonToHuman(reason);
        Debug.LogWarning($"{status}, {message}");
    }

    // Callback for when a new player join the lobby
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player.PlayerId} connected");
        Debug.Log($"Currently in {networkRunner.CurrentScene} scene");

        var lobbyCanvas = GameObject.Find("LobbyCanvas");
        if (lobbyCanvas != null)
        {
            lobbyCanvas.TryGetComponent<LobbyUIScreen>(out LobbyUIScreen lobbyUI);
            lobbyUI.Init();
        }
        else
            Debug.Log("Could not find Lobby Canvas");

        if (runner.IsServer)
        {
            // Server controls the Game Manager
            if (gameMode == GameMode.Host)
                runner.Spawn(gameManagerPrefab, Vector3.zero, Quaternion.identity);

            var roomPlayer = runner.Spawn(roomPlayerPrefab, Vector3.zero, Quaternion.identity, player);
        }
    }

    // Callback for when a player left the lobby
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player {player.PlayerId} disconnected");

        RoomPlayer.Local.PlayerListChanged();

        // Remember to remove the player from the player list in RoomPlayer
        RoomPlayer.RemovePlayer(runner, player);
    }

    // Callback for when the Network Runner shutting down
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log($"Shutting down: {shutdownReason}");

        (string status, string message) = ShutdownReasonToHuman(shutdownReason);

        // Clear the player list
        RoomPlayer.playerList.Clear();

        if (networkRunner)
            Destroy(networkRunner.gameObject);

        networkRunner = null;
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log("Start loading scene");
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        // If it is a game scene, broadcast event for new scene initialize
        Debug.Log($"Scene {runner.CurrentScene} loaded in successfully");

        EventManager.instance.SceneLoaded();
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) { }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }

    // Error message in readable format
    private static (string, string) ShutdownReasonToHuman(ShutdownReason reason)
    {
        switch (reason)
        {
            case ShutdownReason.Ok:
                return (null, null);
            case ShutdownReason.Error:
                return ("Error", "Shutdown was caused by some internal error");
            case ShutdownReason.IncompatibleConfiguration:
                return ("Incompatible Config", "Mismatching type between client Server Mode and Shared Mode");
            case ShutdownReason.ServerInRoom:
                return ("Room name in use", "There's a room with that name! Please try a different name or wait a while.");
            case ShutdownReason.DisconnectedByPluginLogic:
                return ("Disconnected By Plugin Logic", "You were kicked, the room may have been closed");
            case ShutdownReason.GameClosed:
                return ("Game Closed", "The session cannot be joined, the game is closed");
            case ShutdownReason.GameNotFound:
                return ("Game Not Found", "This room does not exist");
            case ShutdownReason.MaxCcuReached:
                return ("Max Players", "The Max CCU has been reached, please try again later");
            case ShutdownReason.InvalidRegion:
                return ("Invalid Region", "The currently selected region is invalid");
            case ShutdownReason.GameIdAlreadyExists:
                return ("ID already exists", "A room with this name has already been created");
            case ShutdownReason.GameIsFull:
                return ("Game is full", "This lobby is full!");
            case ShutdownReason.InvalidAuthentication:
                return ("Invalid Authentication", "The Authentication values are invalid");
            case ShutdownReason.CustomAuthenticationFailed:
                return ("Authentication Failed", "Custom authentication has failed");
            case ShutdownReason.AuthenticationTicketExpired:
                return ("Authentication Expired", "The authentication ticket has expired");
            case ShutdownReason.PhotonCloudTimeout:
                return ("Cloud Timeout", "Connection with the Photon Cloud has timed out");
            default:
                Debug.LogWarning($"Unknown ShutdownReason {reason}");
                return ("Unknown Shutdown Reason", $"{(int)reason}");
        }
    }

    private static (string, string) ConnectFailedReasonToHuman(NetConnectFailedReason reason)
    {
        switch (reason)
        {
            case NetConnectFailedReason.Timeout:
                return ("Timed Out", "");
            case NetConnectFailedReason.ServerRefused:
                return ("Connection Refused", "The lobby may be currently in-game");
            case NetConnectFailedReason.ServerFull:
                return ("Server Full", "");
            default:
                Debug.LogWarning($"Unknown NetConnectFailedReason {reason}");
                return ("Unknown Connection Failure", $"{(int)reason}");
        }
    }
}
