using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyUIScreen : MonoBehaviour
{
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject sessionConfigPanel;
    [SerializeField] private Button startGameButton;

    [SerializeField] private List<PlayerSlotUI> lobbySlots;

    private List<RoomPlayer> playerListCopy = new List<RoomPlayer>();

    private void Start()
    {
        EventManager.instance.onPlayerListUpdated += OnPlayerListUpdated;
    }

    private void OnEnable()
    {
        UIScreen.Focus(GetComponent<UIScreen>());
    }
    
    public void Init()
    {
        lobbyPanel.SetActive(true);
        sessionConfigPanel.SetActive(false);
    }

    private void OnPlayerListUpdated(List<RoomPlayer> playerList)
    {
        // Update the UI for the list of player here
        Debug.Log($"There are now {playerList.Count} player in the lobby");
        playerListCopy = playerList;

        RenderLobby();
    }

    public void RenderLobby()
    {
        if (RoomPlayer.Local != null)
        {
            // Make sure only host of the room can start the game
            if (RoomPlayer.Local.IsLeader)
                startGameButton.interactable = true;
            else
                startGameButton.interactable = false;
        }
        // Toggle playerslot open or closed based on room max size
        for (int i = 0; i < 4; i++)
        {
            lobbySlots[i].GetComponentInChildren<TMP_Text>().text = "Waiting for Player";
            if(i < playerListCopy.Count)
            {
                lobbySlots[i].GetComponentInChildren<TMP_Text>().text = string.IsNullOrEmpty(playerListCopy[i].displayName) ? "Loading..." : playerListCopy[i].displayName;
            }
        }

        // Toggling closed
        for(int j = ServerInfo.MaxUsers; j < ServerInfo.MaxCapacity; j++)
        {
            lobbySlots[j].GetComponentInChildren<TMP_Text>().text = "Unavailable";
        }
    }

    public void StartGame()
    {
        var networkManager = GameObject.FindObjectOfType<NetworkGameManager>();
        if (networkManager != null)
            networkManager.StartGame(SceneIndex.PLAYSCENE);
        else
            Debug.LogError("MISSING NETWORK MANAGER! Please ensure there is an active Network Game Manager in the scene");
    }

    public void QuitLobby()
    {
        var networkManager = GameObject.FindObjectOfType<NetworkGameManager>();
        if (networkManager != null)
            networkManager.LeaveSession();
    }

    private void OnDestroy()
    {
        EventManager.instance.onPlayerListUpdated -= OnPlayerListUpdated;
    }
}
