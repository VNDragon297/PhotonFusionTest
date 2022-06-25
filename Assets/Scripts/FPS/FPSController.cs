using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using UnityEngine.Animations;

[OrderAfter(typeof(NetworkCharacterControllerPrototype))]
public class FPSController : FPSComponent
{
    private NetworkCharacterControllerPrototype playerController;
    private FPSAnimController animController;

    // To be move to setting config file later
    public float walkSpeed = 1.0f;
    public float mouseSens = 1.0f;

    [Networked] private FPSInput.NetworkInputData Inputs { get; set; }
    [Networked] private Vector3 moveDirection { get; set; }
    [Networked] private Vector2 lookDelta { get; set; }
    [Networked] private bool fired { get; set; }

    [Networked(OnChanged = nameof(OnVelocityChangedCallback))]
    private bool isWalking { get; set; }
    [Networked(OnChanged = nameof(OnPlayerAirbornCallback))]
    private bool isGrounded { get; set; }
    public event Action<bool, string> OnBooleanChanged;

    private static void OnPlayerAirbornCallback(Changed<FPSController> changed)
    {
        var newVal = changed.Behaviour.isGrounded;
        changed.LoadOld();
        var oldVal = changed.Behaviour.isGrounded;
        if (newVal != oldVal)
            changed.Behaviour.OnBooleanChanged?.Invoke(!newVal, "isAirborn");
    }

    private static void OnVelocityChangedCallback(Changed<FPSController> changed)
    {
        var newVal = changed.Behaviour.isWalking;
        changed.LoadOld();
        var oldVal = changed.Behaviour.isWalking;
        if(newVal != oldVal)
            changed.Behaviour.OnBooleanChanged?.Invoke(newVal, "isWalking");
    }

    private void Awake()
    {
        playerController = GetComponent<NetworkCharacterControllerPrototype>();
        animController = GetComponent<FPSAnimController>();
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

        isGrounded = playerController.IsGrounded;
    }

    public override void OnGameStart()
    {
        base.OnGameStart();

        // Initiallize things such as audio manager and loggers
    }

    private Vector3 MoveAxisRemap(Vector2 controllerInput) => new Vector3(controllerInput.x, 0, controllerInput.y);

    private void Move(FPSInput.NetworkInputData inputs)
    {
        // Player move in a 3D space, therefore must remap movement to 3D space
        moveDirection = MoveAxisRemap(inputs.moveDirection);
        Vector3 move = transform.right * moveDirection.x + transform.forward * moveDirection.z;
        playerController.Move(move * walkSpeed * Runner.DeltaTime);

        if (playerController.IsGrounded)
            isWalking = (moveDirection.z >= .125f);

        if (inputs.isJumpPressed)
            Jump();
    }

    [Header("Camera Position")]
    public Transform headTransform;
    float xRotation = 0f;
    private void Look(FPSInput.NetworkInputData inputs)
    {
        // Using runner.Deltatime might be bad unless you have client prediction
        float mouseX = inputs.lookDelta.x * mouseSens * Runner.DeltaTime;
        float mouseY = inputs.lookDelta.y * mouseSens * Runner.DeltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Currently broken on clients
        transform.Rotate(mouseX * Vector3.up);
        headTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void Jump()
    {
        playerController.Jump(false);
    }
}
