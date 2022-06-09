using Fusion.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance => Singleton<EventManager>.Instance;

    // Sample event rig
    public Action onChangedInvokation;
    public void ChangedInvokation()
    {
        if (onChangedInvokation != null)
            onChangedInvokation();
    }

    public Action onPlayerJoined;
    public void PlayerJoined()
    {
        if (onPlayerJoined != null)
            onPlayerJoined();
    }

    public Action<List<RoomPlayer>> onPlayerListUpdated;
    public void PlayerListUpdated(List<RoomPlayer> list)
    {
        if (onPlayerListUpdated != null)
            onPlayerListUpdated(list);
    }

    public Action onSceneLoaded;
    public void SceneLoaded()
    {
        if(onSceneLoaded != null)
        {
            onSceneLoaded();
        }
    }

    public Action<Transform> onCamBasePositionUpdate;
    public void UpdateCameraBasePosition(Transform pos)
    {
        if (onCamBasePositionUpdate != null)
            onCamBasePositionUpdate(pos);
    }

    public Action<float, float> onViewportRotate;
    public void UpdateViewportRotation(float xRot, float yRot)
    {
        if (onViewportRotate != null)
            onViewportRotate(xRot, yRot);
    }
}
