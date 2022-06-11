using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class FPSAnimController : FPSComponent
{
    [SerializeField] private NetworkMecanimAnimator networkAnimator;
    [SerializeField] private Animator animator;

    private FPSController controller;
    private CharacterController charController;

    public override void Spawned()
    {
        base.Spawned();
        controller = GetComponent<FPSController>();
        charController = GetComponent<CharacterController>();

        // State change events hooks goes here
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
    }

    // Network Mecanim Animator only seems to work with syncing triggers
    public void SetTrigger(string name)
    {
        if (Object.HasStateAuthority)
            networkAnimator.SetTrigger(name);
        else if (Object.HasInputAuthority)
            animator.SetTrigger(name);
    }
}
