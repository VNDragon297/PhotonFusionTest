using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;
using Fusion.Sockets;

public class FPSInput : FPSComponent, INetworkRunnerCallbacks
{
    public struct NetworkInputData : INetworkInput
    {
        public const byte MOUSEBUTTON0 = 0x01;      // LMB
        public const byte MOUSEBUTTON1 = 0x02;      // RMB

        public byte buttons;
        public Vector2 moveDirection;
        public Vector2 lookDelta;
        public bool fired;
    }

    public Gamepad gamepad;

    [SerializeField] private InputAction Move;
    [SerializeField] private InputAction Look;
    [SerializeField] private InputAction Fire;

    public override void Spawned()
    {
        base.Spawned();
        Runner.AddCallbacks(this);

        // Clone create an identical input map with a new unique identifier
        // Input events will not be clone/copy over
        Move = Move.Clone();
        Look = Look.Clone();
        Fire = Fire.Clone();

        Move.Enable();
        Look.Enable();
        Fire.Enable();

        Fire.performed += FirePressed;
    }

    private void FirePressed(InputAction.CallbackContext ctx)
    {

    }

    private void DisposeInput()
    {
        Move.Dispose();
        Look.Dispose();
        Fire.Dispose();
        // Does not need to unsubscribe from input events as
        // Dispose function will take care of that
    }

    private void OnDestroy()
    {
        DisposeInput();
    }

    #region Network Callbacks
    #region Inputs
    private static Vector2 ReadVector2(InputAction action) => action.ReadValue<Vector2>();
    private static bool ReadBoolean(InputAction action) => action.ReadValue<float>() != 0;

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        // Initiallize game controller and inputs
        gamepad = Gamepad.current;
        var userInput = new NetworkInputData();

        // Read inputs
        userInput.moveDirection = ReadVector2(Move);
        userInput.lookDelta = ReadVector2(Look);
        userInput.fired = ReadBoolean(Fire);

        // Send user's input to server
        input.Set(userInput);
    }
    #endregion

    #region Unused
    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }
    #endregion
#endregion
}
