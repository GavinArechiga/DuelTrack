using System;
using UnityEngine;

public class CleanupTool : Tool
{
    [SerializeField] private CleanupToolInputReaderSO inputReader;
    [SerializeField] private GameObject broomPrefab;
    [SerializeField] private Vector3 broomOffset;
    public static event Action<bool> OnMountToggled;
    
    private bool mounted;
    private GameObject broomInstance;

    private void Awake()
    {
        inputReader.OnToggleMount += HandleToggleMount;
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
        
        if (mounted)
        {
            broomInstance = Instantiate(broomPrefab, playerController.transform.position, playerController.transform.rotation);
            
            Vector3 broomPosition = broomInstance.transform.position + broomOffset;
            broomInstance.transform.position = broomPosition;
            
            playerController.transform.position = broomInstance.transform.position;
            broomInstance.transform.SetParent(playerController.transform, true);
        }
        else if (broomInstance != null)
        {
            broomInstance.transform.SetParent(null);
            Destroy(broomInstance);
        }
        
        OnMountToggled?.Invoke(mounted);
    }
}
