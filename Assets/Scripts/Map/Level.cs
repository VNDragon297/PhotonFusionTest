using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Level : NetworkBehaviour
{
    public static Level current { get; private set; }

    [Networked] public TickTimer MapTimer { get; set; }

    public Transform[] spawnPoints;

    private void Awake()
    {
        current = this;

        GameManager.instance.camera = Camera.main;
    }

    public override void Spawned()
    {
        base.Spawned();
    }

    // Placeholder prefab selection
    [SerializeField] private NetworkPrefabRef playerPrefab;
    public void SpawnPlayers(NetworkRunner runner, RoomPlayer player)
    {
        var index = RoomPlayer.playerList.IndexOf(player);
        var spawnPoint = spawnPoints[0];

        // Modify this if player is customizable
        // var prefabId = player.PrefabId;
        // Find prefab base on prefabId

        var entity = runner.Spawn(
                playerPrefab,
                spawnPoint.position,
                spawnPoint.rotation,
                player.Object.InputAuthority
            );

        if(entity != null)
        {
            Debug.Log($"Spawning entity {entity.name} for {player.displayName}");
            entity.transform.name = player.displayName;
        }
        else
        {
            Debug.LogWarning("Entity not fully spawned");
        }
    }
}
