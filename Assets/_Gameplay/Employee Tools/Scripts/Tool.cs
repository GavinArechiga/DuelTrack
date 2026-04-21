using System;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Tool : MonoBehaviour
{
    public ToolType toolType;
    protected PlayerController playerController;
    protected PlayerVisual playerVisual;

    public void Initialize(PlayerController playerController, PlayerVisual playerVisual)
    {
        this.playerController = playerController;
        this.playerVisual = playerVisual;
    }

    public abstract void Enter();
    public abstract void Exit();
    public abstract void ToolUpdate();
}
