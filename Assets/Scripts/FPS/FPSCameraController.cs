using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class FPSCameraController : FPSComponent, ICameraController
{
    public Transform camNode;
    public Transform activeNode;

    public float mouseSens = 1.0f;

    private void Awake()
    {
        if (activeNode == null)
            activeNode = camNode;

        if (GameManager.instance.Runner.IsClient)
        {
            GameManager.GetCameraControl(this);
        }
    }

    public override void Render()
    {
        base.Render();

        if (Object.HasInputAuthority && !GameManager.IsCameraControlled)
            GameManager.GetCameraControl(this);
    }

    public bool ControlCamera(Camera cam)
    {
        if(this.Equals(null))
        {
            Debug.LogWarning("Releasing camera from player");
            return false;
        }

        CameraPositionLerp(cam);
        return true;
    }

    private void CameraPositionLerp(Camera cam)
    {
        cam.transform.position = Vector3.Lerp(cam.transform.position, camNode.position, Time.deltaTime * 100f);
        cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, camNode.rotation, Time.deltaTime * 100f);
    }
}
