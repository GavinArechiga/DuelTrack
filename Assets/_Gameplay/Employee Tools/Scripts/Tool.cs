using System;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Tool : MonoBehaviour
{
    public ToolType toolType;
    protected PlayerController playerController;

    public void Initialize(PlayerController playerController)
    {
        this.playerController = playerController;
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void ToolUpdate();
}
