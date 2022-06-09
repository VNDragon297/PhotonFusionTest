using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class FPSCameraController : CameraControllerBase
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

        EventManager.instance.onCamBasePositionUpdate += FollowPosition;
        EventManager.instance.onViewportRotate += RotateCamera;
    }

    private void FollowPosition(Transform pos)
    {
        this.transform.position = pos.position;
    }

    public void RotateCamera(float xRot, float yRot)
    {
        var cam = GetComponentInChildren<Camera>();

        if(cam != null)
            cam.transform.localRotation = Quaternion.Euler(xRot * mouseSens, 0f, 0f);
        transform.Rotate(yRot * Vector3.up);      // Currently rotating the body of the mesh, but not rotating the camera as camera is not attached to mesh directly
    }

    private void OnDestroy()
    {
        EventManager.instance.onCamBasePositionUpdate -= FollowPosition;
        EventManager.instance.onViewportRotate -= RotateCamera;
    }
}
