using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

[OrderAfter(typeof(NetworkCharacterControllerPrototype))]
public class FPSController : FPSComponent
{
    private NetworkCharacterControllerPrototype playerController;

    // To be move to setting config file later
    public float walkSpeed = 1.0f;
    public float mouseSens = 1.0f;

    [Networked] private FPSInput.NetworkInputData Inputs { get; set; }
    [Networked] private Vector3 moveDirection { get; set; }
    [Networked] private Vector2 lookDelta { get; set; }
    [Networked] private bool fired { get; set; }

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

        Look(Inputs);
        Move(Inputs);
    }

    public override void OnGameStart()
    {
        base.OnGameStart();

        // Initiallize things such as audio manager and loggers
    }

    private Vector3 MoveAxisRemap(Vector2 controllerInput) => new Vector3(controllerInput.x, 0, controllerInput.y);

    private void Move(FPSInput.NetworkInputData inputs)
    {
        if (playerController.IsGrounded)
        {
            // Player move in a 3D space, therefore must remap movement to 3D space
            moveDirection = MoveAxisRemap(inputs.moveDirection);
            Vector3 move = transform.right * moveDirection.x + transform.forward * moveDirection.z;
            playerController.Move(moveDirection * walkSpeed * Runner.DeltaTime);
        }
        else
        {
            playerController.Move(Vector3.zero);        // Move function is responsible for character controller falling as well
        }
    }

    public Transform headRotation;
    float xRotation = 0f;
    private void Look(FPSInput.NetworkInputData inputs)
    {
        if (Object.HasInputAuthority)
        {
            // Using runner.Deltatime might be bad unless you have client prediction
            float mouseX = inputs.lookDelta.x * mouseSens * Runner.DeltaTime;
            float mouseY = inputs.lookDelta.y * mouseSens * Runner.DeltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            headRotation.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }
}
