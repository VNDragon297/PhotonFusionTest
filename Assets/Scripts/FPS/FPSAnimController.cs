using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class FPSAnimController : FPSComponent
{
    [SerializeField] private Animator animator;

    private FPSController controller;
    private CharacterController charController;

    public override void Spawned()
    {
        base.Spawned();
        controller = GetComponent<FPSController>();
        charController = GetComponent<CharacterController>();

        // State change events hooks goes here
        controller.OnVelocityChanged += SetBool;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
    }

    private void SetBool(bool val, string name) => animator.SetBool(name, val);

    private void OnDestroy()
    {
        controller.OnVelocityChanged -= SetBool;
    }
}
