using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerBase : MonoBehaviour
{
    public enum ControllerType
    {
        NONE,
        FPS,
        TPS,
        TOPDOWN,
        GROUP
    }

    public ControllerType cType;

    public virtual void SetDefaultCamPosition(Camera cam) { }
    public virtual bool ControlCamera(Camera cam)
    {
        if (this.Equals(null))
        {
            Debug.LogWarning("Releasing camera from player");
            return false;
        }

        return true;
    }

}
