using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Utility;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance => Singleton<GameManager>.Instance;

    private CameraControllerBase cameraController;
    public new Camera camera;

    public static Level currentLevel { get; private set; }
    public static bool isPlayer => currentLevel != null;

    [Networked(OnChanged = nameof(OnLobbyDetailsChanged))] public string LobbyName { get; set; }
    [Networked(OnChanged = nameof(OnLobbyDetailsChanged))] public int MaxPlayers { get; set; }

    private static void OnLobbyDetailsChanged(Changed<GameManager> changed)
    {
        // Invoke an event or do something here
    }
    public static bool IsCameraControlled => instance.cameraController != null;
    public static void GetCameraControl(CameraControllerBase controller) => instance.cameraController = controller;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public override void Spawned()
    {
        base.Spawned();

        // Set network variables if this is the server/host
        if(Object.HasStateAuthority)
        {
            LobbyName = ServerInfo.LobbyName;
            MaxPlayers = ServerInfo.MaxUsers;
        }
    }

    public static void SetLevel(Level level)
    {
        currentLevel = level;
    }

    private void LateUpdate()
    {
        if (cameraController == null) return;
        if(cameraController.Equals(null))
        {
            Debug.LogWarning("Ghost object for camera controller");
            cameraController = null;
            return;
        }

        if (cameraController.ControlCamera(camera) == false)
            cameraController = null;
    }
}
