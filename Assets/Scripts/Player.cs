using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using TMPro;

public class Player : NetworkBehaviour
{
    private NetworkCharacterControllerPrototype _characterController;

    [Networked(OnChanged = nameof(OnBallSpawned))]
    public NetworkBool spawned { get; set; }
    private Material _material;
    Material material
    {
        get
        {
            if (_material == null)
                _material = GetComponentInChildren<MeshRenderer>().material;
            return _material;
        }
    }

    [Networked] TickTimer delay { get; set; }
    [SerializeField] private Ball _prefabBall;
    [SerializeField] private PhysXBall _prefabPhysXBall;
    private Vector3 _forward;

    private void Awake()
    {
        _characterController = GetComponent<NetworkCharacterControllerPrototype>();
        _forward = transform.forward;
    }

    private void Update()
    {
        if(Object.HasInputAuthority && Input.GetKeyDown(KeyCode.Space))
        {
            RPC_SendMessage("Hello World!");
        }
    }

    private TMP_Text _messages;
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_SendMessage(string msg, RpcInfo info = default)
    {
        if (_messages == null)
            _messages = FindObjectOfType<TMP_Text>();
        if (info.Source == Runner.Simulation.LocalPlayer)
            msg = $"You: {msg}\n";
        else
            msg = $"Someone else: {msg}\n";

        _messages.text += msg;
    }

    public override void Render()
    {
        base.Render();

        material.color = Color.Lerp(material.color, Color.blue, Time.deltaTime);
    }

    public override void FixedUpdateNetwork()
    {
        if(GetInput(out SampleNetworkInputData data))
        {
            data.direction.Normalize();
            _characterController.Move(5 * data.direction * Runner.DeltaTime);

            if(data.direction.sqrMagnitude > 0)
                _forward = data.direction;

            if(delay.ExpiredOrNotRunning(Runner))
            {
                if ((data.buttons & SampleNetworkInputData.MOUSEBUTTON1) != 0)
                {
                    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                    Runner.Spawn(
                        _prefabBall,
                        transform.position + _forward,
                        Quaternion.identity,
                        Object.InputAuthority,
                        (runner, obj) =>
                        {
                            // Initialize functions call here before syncing
                            obj.GetComponent<Ball>().Init();
                        }
                        );
                    spawned = !spawned;
                }

                if ((data.buttons & SampleNetworkInputData.MOUSEBUTTON2) != 0)
                {
                    delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                    Runner.Spawn(
                        _prefabPhysXBall,
                        transform.position + _forward,
                        Quaternion.identity,
                        Object.InputAuthority,
                        (runner, obj) =>
                        {
                            // Initialize functions call here before syncing
                            obj.GetComponent<PhysXBall>().Init(10 * _forward);
                        }
                        );
                    spawned = !spawned;
                }
            }
        }
    }

    private static void OnBallSpawned(Changed<Player> changed)
    {
        changed.Behaviour.material.color = Color.white;
    }
}
