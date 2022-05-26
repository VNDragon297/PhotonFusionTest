using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

[OrderAfter(typeof(NetworkCharacterControllerPrototype))]
public class FPSController : FPSComponent
{
    private NetworkCharacterControllerPrototype playerController;

    public float walkSpeed = 1.0f;

    [Networked] private FPSInput.NetworkInputData Inputs { get; set; }

    private void Awake()
    {
        playerController = GetComponent<NetworkCharacterControllerPrototype>();
    }

    public override void Spawned()
    {
        base.Spawned();
    }

    private void Update()
    {
        if(Object.HasInputAuthority)
        {
            if(Entity.input.gamepad != null)
            {
                // Set gamepad settings here
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if(GetInput(out FPSInput.NetworkInputData input))
        {
            // Copy inputs receiced, to a [Networked] property, so other clients can
            // predict tick-aligned inputs. This is the core of Client Prediction system
            Inputs = input;
        }

        if (playerController.IsGrounded)
            Move(Inputs);
    }

    public override void OnGameStart()
    {
        base.OnGameStart();

        // Initiallize things such as audio manager and loggers
    }

    private void Move(FPSInput.NetworkInputData inputs)
    {
        if(Object.HasInputAuthority)
        {
            Debug.Log($"Direction: {inputs.direction}");
        }
    }
}
