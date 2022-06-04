using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICameraController
{
    bool ControlCamera(Camera cam);
    void FollowNode(Camera cam);
    void RotateCamera(Camera cam, float xRot);
}
