using System;
using UnityEngine;

public class CleanupTool : Tool
{
    [SerializeField] private CleanupToolInputReaderSO inputReader;
    [SerializeField] private GameObject broomGameObject;
    [SerializeField] private LayerMask trashLayerMask;
    public static event Action<bool> OnToggleMount;
    
    private bool mounted;

    private void Awake()
    {
        inputReader.OnToggleMount += HandleToggleMount;
        broomGameObject.SetActive(false);
    }

    public override void Enter()
    {
        inputReader.EnableInput();
    }

    public override void Exit()
    {
        inputReader.DisableInput();
        if (mounted)
        {
            HandleToggleMount();
        }
    }

    public override void ToolUpdate()
    {
        
    }
    
    private void HandleToggleMount()
    {
        mounted = !mounted;
        broomGameObject.SetActive(mounted);
        OnToggleMount?.Invoke(mounted);
    }
}
