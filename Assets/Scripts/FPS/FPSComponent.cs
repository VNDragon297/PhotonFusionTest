using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class FPSComponent : NetworkBehaviour
{
    public FPSEntity Entity { get; private set; }

    public virtual void Init(FPSEntity entity)
    {
        Entity = entity;
    }

    // Called on the tick when game has started. Method is tick-aligned
    public virtual void OnGameStart() { }
}