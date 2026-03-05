using System;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Tool : MonoBehaviour, ITool
{
    public ToolManager.ToolType toolType;
    protected PlayerController playerController;

    public void Initialize(PlayerController playerController)
    {
        this.playerController = playerController;
        enabled = false;
    }

    public virtual void Enter()
    {
        enabled = true;
    }

    public virtual void Exit()
    {
        enabled = false;
    }
}
