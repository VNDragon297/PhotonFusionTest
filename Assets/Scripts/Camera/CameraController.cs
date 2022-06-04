using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CameraController : FPSComponent, ICameraController
{
    public Transform camNode;
    public Transform activeNode;

    public float mouseSens = 1.0f;

    private void Awake()
    {
        if (activeNode == null)
            activeNode = camNode;
    }

    public override void Render()
    {
        base.Render();

        if(Object.HasInputAuthority && !GameManager.IsCameraControlled)
        {
            GameManager.GetCameraControl(this);
        }
    }

    public bool ControlCamera(Camera cam)
    {
        if(this.Equals(null))
        {
            Debug.LogWarning("Releasing camera from player");
            return false;
        }

        return true;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();


    }

    public void FollowNode(Camera cam)
    {
        cam.transform.position = activeNode.position;
    }

    public void RotateCamera(Camera cam, float xRot)
    {
        cam.transform.localRotation = Quaternion.Euler(xRot * mouseSens, 0f, 0f);
    }
}
