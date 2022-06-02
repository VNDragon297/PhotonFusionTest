using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSEntity : FPSComponent
{
    public FPSController controller { get; private set; }
    public FPSInput input { get; private set; }

    public CameraController camera { get; private set; }

    private void Awake()
    {
        controller = GetComponent<FPSController>();
        input = GetComponent<FPSInput>();
        camera = GetComponent<CameraController>();

        // Initiallize all FPSComponent in children objects
        var components = GetComponentsInChildren<FPSComponent>();
        foreach(var component in components)
        {
            component.Init(this);
        }
    }

    public static readonly List<FPSEntity> Players = new List<FPSEntity>();

    public override void Spawned()
    {
        base.Spawned();

        if(Object.HasInputAuthority)
        {
            // Do what you need to do with local player here

            // Create HUD for local player
        }

        // Keep track of FPS players available
        Players.Add(this);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);

        Players.Remove(this);
    }

    private void OnDestroy()
    {
        Players.Remove(this);
    }
}
