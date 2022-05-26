using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class RoomPlayer : NetworkBehaviour
{
    // Keep track of all the players in the room
    public static readonly List<RoomPlayer> playerList = new List<RoomPlayer>();

    public static RoomPlayer Local;

    // Need to be change if game is running on a dedicated server, as no one will be host
    // State Authority will only be given to host
    public bool IsLeader => Object != null && Object.IsValid && Object.HasStateAuthority;

    [Networked(OnChanged = nameof(OnAttributeChanged))] public NetworkBool isReady { get; set; }
    [Networked(OnChanged = nameof(OnAttributeChanged))] public string displayName { get; set; }

    public override void Spawned()
    {
        base.Spawned();

        if (Object.HasInputAuthority)
        {
            Local = this;

            // There is a change in player, need to update
            RPC_SetPlayerInfo(PlayerInfo.displayName);
        }

        playerList.Add(this);
        EventManager.instance.PlayerListUpdated(playerList);

        DontDestroyOnLoad(gameObject);
    }

    // Player with the authority over object's input sent RPC message to server
    // SetPlayerInfo will be call everytime that there is a change in player's information
    // If an information is not change, it will skip updating that piece of information
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority, InvokeResim = true)]
    private void RPC_SetPlayerInfo(string _displayName = "")
    {
        // Update info if there was a change
        if(!string.IsNullOrEmpty(_displayName))
            displayName = _displayName;
    }

    public static void OnAttributeChanged(Changed<RoomPlayer> changed)
    {
        // Invoke an event or do something here
        var LobbyUIManager = GameObject.FindObjectOfType<LobbyUIScreen>();
        if (LobbyUIManager != null)
            LobbyUIManager.RenderLobby();
        Debug.Log("Room player attribute changed");
    }
    
    public void PlayerListChanged()
    {
        if (EventManager.instance != null)
            EventManager.instance.PlayerListUpdated(playerList);
    }

    // If player is on the list, remove the player
    public static void RemovePlayer(NetworkRunner _runner, PlayerRef _player)
    {
        var roomPlayer = playerList.FirstOrDefault(x => x.Object.InputAuthority == _player);
        if(roomPlayer != null)
        {
            playerList.Remove(roomPlayer);
            _runner.Despawn(roomPlayer.Object);
            EventManager.instance.PlayerListUpdated(playerList);
        }
    }
}
