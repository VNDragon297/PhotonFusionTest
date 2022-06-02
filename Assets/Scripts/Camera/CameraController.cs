using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : FPSComponent, ICameraController
{
    public Transform camNode;

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
}
