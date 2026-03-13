using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConstructionTool : Tool
{
    [SerializeField] private ConstructionToolInputReaderSO inputReader;

    private void Awake()
    {
        inputReader.OnPlaceObjectPerformed += PlaceObject;
        inputReader.OnRemoveObjectPerformed += RemoveObject;
    }

    private void OnDestroy()
    {
        inputReader.OnPlaceObjectPerformed -= PlaceObject;
        inputReader.OnRemoveObjectPerformed -= RemoveObject;
    }

    public override void Enter()
    {
        inputReader.EnableInput();
        GridSystem.Instance.EnableGrid(true);
    }
    
    public override void Exit()
    {
        inputReader.DisableInput();
        GridSystem.Instance.EnableGrid(false);
    }

    public override void ToolUpdate()
    {
        GridSystem.Instance.UpdateGrid(transform.position, 
            playerController.CurrentFacingDirection);
    }
    
    private void PlaceObject()
    {
        GridSystem.Instance.PlaceObject();
    }
    
    private void RemoveObject()
    {
        GridSystem.Instance.RemoveObject();
    }
}
