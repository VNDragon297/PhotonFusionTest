using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Utility;
using UnityEngine.SceneManagement;

public class LevelManager : NetworkSceneManagerBase
{
    // Test to see if can use dummyScreen as loading screen
    [SerializeField] private UIScreen dummyScreen;      // Use screen used for navigation stack between transition to prevent UI feature from breaking
    [SerializeField] private UIScreen lobbyScreen;

    public static LevelManager instance => Singleton<LevelManager>.Instance;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public static void LoadMenu()
    {
        instance.Runner.SetActiveScene((int)SceneIndex.MAINMENU);
    }

    public static void LoadScene(SceneIndex index)
    {
        instance.Runner.SetActiveScene((int)index);
    }

    protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
    {
        Debug.Log($"Loading scene {newScene}");

        PreLoadScene(newScene);

        List<NetworkObject> sceneObjects = new List<NetworkObject>();

        if(newScene >= (int)SceneIndex.MAINMENU)
        {
            yield return SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Single);
            Scene loadedScene = SceneManager.GetSceneByBuildIndex(newScene);
            Debug.Log($"Loaded scene {newScene}: {loadedScene}");
            sceneObjects = FindNetworkObjects(loadedScene, disable: false);
        }

        finished(sceneObjects);

        //Delay one frame, to make sure level objects spawned locally
        yield return null;

        // Spawn players
        if(newScene != null && newScene > (int)SceneIndex.MAINMENU)
        {
            if(Runner.GameMode == GameMode.Host || Runner.GameMode == GameMode.Server)
            {
                foreach(var player in RoomPlayer.playerList)
                {
                    Debug.Log($"Attempting to spawn {player.displayName}");
                    GameManager.currentLevel.SpawnPlayers(Runner, player);
                }
            }
        }
    }

    private void PreLoadScene(int scene)
    {
        if(scene > (int)SceneIndex.MAINMENU)
        {
            Debug.Log("Showing dummy screen");
            UIScreen.Focus(dummyScreen);
        }
        else if(scene == (int)SceneIndex.MAINMENU)
        {
            UIScreen.activeScreen.BackTo(lobbyScreen);
        }
    }
}
